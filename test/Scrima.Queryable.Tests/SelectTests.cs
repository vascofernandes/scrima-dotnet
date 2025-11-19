using System.Linq;
using System.Collections.Generic;
using FluentAssertions;
using Scrima.Core;
using Scrima.Core.Model;
using Scrima.Core.Query;
using Scrima.Core.Query.Expressions;
using Xunit;

namespace Scrima.Queryable.Tests;

public class SelectTests
{
    private readonly EdmComplexType _edmType;
    private readonly IQueryable<TestModel> _queryable;

    public SelectTests()
    {
        var elementsList = new List<TestModel>
        {
            new TestModel
            {
                Id = 1,
                Name = "Joe",
                NestedModel = new NestedModel {Id = 99, Name = "Element 99"},
                NestedList = new List<NestedModel>
                {
                    new NestedModel {Id = 1, Name = "Element 1"},
                    new NestedModel {Id = 2, Name = "Element 2"}
                }
            },
            new TestModel
            {
                Id = 2,
                Name = "Mark",
                NestedModel = new NestedModel {Id = 98, Name = "Element 98"},
                NestedList = new List<NestedModel>
                {
                    new NestedModel {Id = 3, Name = "Element 3"},
                    new NestedModel {Id = 4, Name = "Element 4"}
                }
            }
        };

        var provider = new EdmTypeProvider();
        _edmType = provider.GetByClrType(typeof(TestModel)) as EdmComplexType;
        _queryable = elementsList.AsQueryable();
    }

    [Fact]
    public void Should_select_nested_property()
    {
        // $select=NestedModel
        var nestedModelProp = _edmType.GetProperty(nameof(TestModel.NestedModel));
        
        var query = new QueryOptions(
            _edmType,
            new SelectQueryOption(new PropertyAccessNode(new[]
            {
                nestedModelProp
            })),
            new FilterQueryOption(null),
            new OrderByQueryOption(Enumerable.Empty<OrderByProperty>()),
            null,
            0,
            null,
            10,
            true
        );

        var results = _queryable.ToQueryResult(query);

        results.Results.Should().HaveCount(2);
        results.Results.First().NestedModel.Should().NotBeNull();
        results.Results.First().NestedModel.Name.Should().Be("Element 99");
    }
    
    [Fact]
    public void Should_select_nested_property_leaf()
    {
        // $select=NestedModel/Name
        var nestedModelProp = _edmType.GetProperty(nameof(TestModel.NestedModel));
        var nestedType = nestedModelProp.PropertyType as EdmComplexType;
        var nameProp = nestedType.GetProperty(nameof(NestedModel.Name));
        
        var query = new QueryOptions(
            _edmType,
            new SelectQueryOption(new PropertyAccessNode(new[]
            {
                nestedModelProp,
                nameProp
            })),
            new FilterQueryOption(null),
            new OrderByQueryOption(Enumerable.Empty<OrderByProperty>()),
            null,
            0,
            null,
            10,
            true
        );

        var results = _queryable.ToQueryResult(query);

        results.Results.Should().HaveCount(2);
        results.Results.First().NestedModel.Should().NotBeNull();
        results.Results.First().NestedModel.Name.Should().Be("Element 99");
        results.Results.First().NestedModel.Id.Should().Be(0); // Not selected
    }

    [Fact]
    public void Should_select_collection_property()
    {
        // $select=NestedList
        var nestedListProp = _edmType.GetProperty(nameof(TestModel.NestedList));
        
        var query = new QueryOptions(
            _edmType,
            new SelectQueryOption(new PropertyAccessNode(new[]
            {
                nestedListProp
            })),
            new FilterQueryOption(null),
            new OrderByQueryOption(Enumerable.Empty<OrderByProperty>()),
            null,
            0,
            null,
            10,
            true
        );

        var results = _queryable.ToQueryResult(query);

        results.Results.Should().HaveCount(2);
        var first = results.Results.First();
        first.NestedList.Should().NotBeNull();
        first.NestedList.Should().HaveCount(2);
        first.NestedList.First().Name.Should().Be("Element 1");
    }

    public class TestModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public NestedModel NestedModel { get; set; }
        public List<NestedModel> NestedList { get; set; }
    }

    public class NestedModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
