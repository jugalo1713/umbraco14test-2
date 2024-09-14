using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Api.Common.Attributes;
using Umbraco.Cms.Api.Common.ViewModels.Pagination;
using Umbraco.Cms.Api.Management.Controllers;
using Umbraco.Cms.Api.Management.Routing;

namespace umbraco14test_2.controllers;

[VersionedApiBackOfficeRoute("my/item")]
[ApiExplorerSettings(GroupName = "My item API")]
public class MyItemApiController : ManagementApiControllerBase
{
	private static readonly List<MyItem> AllItems = Enumerable.Range(1, 100)
		.Select(i => new MyItem($"My Item #{i}"))
		.ToList();

	[HttpGet]
	public IActionResult GetAllItems(int skip = 0, int take = 10)
		=> Ok(
			new PagedViewModel<MyItem>
			{
				Items = AllItems.Skip(skip).Take(take),
				Total = AllItems.Count
			}
		);

	[HttpGet("{id:guid}")]
	public IActionResult GetItem(Guid id)
	{
		MyItem? item = AllItems.FirstOrDefault(item => item.Id == id);

		return item is not null
			? Ok(item)
			: OperationStatusResult(
				MyItemOperationStatus.NotFound,
				builder => NotFound(
					builder
						.WithTitle("That thing wasn't there")
						.WithDetail("Maybe look for something else?")
						.Build()
				)
			);
	}

	[HttpPost]
	public IActionResult CreateItem(string value)
	{
		if (value.StartsWith("New") is false)
		{
			return OperationStatusResult(
				MyItemOperationStatus.InvalidValue,
				builder => BadRequest(
					builder
						.WithTitle("That was invalid")
						.Build()
				)
			);
		}

		if (AllItems.Any(item => item.Value.InvariantEquals(value)))
		{
			return OperationStatusResult(
				MyItemOperationStatus.DuplicateValue,
				builder => BadRequest(
					builder
						.WithTitle("No duplicate values, please.")
						.Build()
				)
			);
		}

		var newItem = new MyItem(value);
		AllItems.Add(newItem);
		return CreatedAtId<MyItemApiController>(
			ctrl => nameof(ctrl.GetItem),
			newItem.Id
		);
	}

	[HttpPut("{id:guid}")]
	public IActionResult UpdateItem(Guid id, string value)
	{
		MyItem? item = AllItems.FirstOrDefault(item => item.Id == id);
		if (item is null)
		{
			return OperationStatusResult(
				MyItemOperationStatus.NotFound,
				builder => NotFound(
					builder
						.WithTitle("That thing wasn't there")
						.WithDetail("Maybe look for something else?")
						.Build()
				)
			);
		}

		if (AllItems.Any(i => i.Value.InvariantEquals(value)))
		{
			return OperationStatusResult(
				MyItemOperationStatus.DuplicateValue,
				builder => BadRequest(
					builder
						.WithTitle("You have been DUPED")
						.Build()
				)
			);
		}

		item.Value = value;
		return Ok();
	}

	[HttpDelete("{id:guid}")]
	public IActionResult DeleteItem(Guid id)
	{
		MyItem? item = AllItems.FirstOrDefault(item => item.Id == id);

		if (item is null)
		{
			return OperationStatusResult(
				MyItemOperationStatus.NotFound,
				builder => NotFound(
					builder
						.WithTitle("That thing wasn't there")
						.WithDetail("Maybe look for something else?")
						.Build()
				)
			);
		}

		AllItems.Remove(item);
		return Ok();
	}
}

public class MyItem(string value)
{
	public Guid Id { get; } = Guid.NewGuid();

	public string Value { get; set; } = value;
}

public enum MyItemOperationStatus
{
	NotFound,
	InvalidValue,
	DuplicateValue
}