using CoreMapper;
using CoreMapper.Entities;
using CoreMapper.Entities.Strategies;
using CoreMapper.Strategies;
using CoreMapper.Tests.Models;
using ObjectChangeTracking;
using ObjectChangeTracking.CoreMapper;
using ObjectChangeTracking.CoreMapper.Strategies;
using System;
using System.Linq;
using System.Linq.Expressions;
using Xunit;

namespace CoreMapper.Tests
{
    public class Test
    {
        protected IMapper CreateMapper()
        {
            IEntitySelector entitySelector = new EntitySelector();

            Mapper mapper = new Mapper();
            mapper.MappingStrategies.Add(new SameNameAndTypeStrategy());
            mapper.MappingStrategies.Add(new EntityToIdStrategry());
            mapper.MappingStrategies.Add(new IdToEntityStrategy(entitySelector));
            mapper.MappingStrategies.Add(new CollectionToAddedStrategy());
            mapper.Interceptors.Add(new ChangeTrackingInterceptor());

            return mapper;
        }

        [Fact]
        public void EqualProperties()
        {
            try
            {
                ItemA itemA = new ItemA() { Value = "123", Counter = 12 };
                ItemB itemB = new ItemB() { Value = "...", Counter = 0 };

                CreateMapper().Map(itemA, itemB);

                Assert.Equal("123", itemB.Value);
                Assert.Equal(12, itemB.Counter);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        [Fact]
        public void NullableToNonNullable()
        {
            ItemA objA = new ItemA() { IsActive = true };
            ItemB objB = new ItemB() { IsActive = false };

            CreateMapper().Map(objA, objB);
            
            Assert.True(objB.IsActive);
        }

        [Fact]
        public void NullableToNonNullableWithNullValue()
        {
            ItemA objA = new ItemA() { IsActive = null };
            ItemB objB = new ItemB() { IsActive = true };

            CreateMapper().Map(objA, objB);

            Assert.True(objB.IsActive);
        }

        [Fact]
        public void NonNullableToNullable()
        {
            ItemB objA = new ItemB() { IsActive = true };
            ItemA objB = new ItemA() { IsActive = false };

            CreateMapper().Map(objA, objB);

            Assert.True(objB.IsActive);
        }

        [Fact]
        public void NonNullableToNullableWithNullValue()
        {
            ItemB objA = new ItemB() { IsActive = true };
            ItemA objB = new ItemA() { IsActive = null };

            CreateMapper().Map(objA, objB);

            Assert.True(objB.IsActive);
        }

        [Fact]
        public void ChangeTracking()
        {
            ItemB objA = new ItemB() { Counter = 101 }.AsTrackable();
            objA.IsActive = true;

            ItemA objB = new ItemA() { IsActive = null };

            CreateMapper().Map(objA, objB);

            Assert.True(objB.IsActive);  //changed
            Assert.True(objB.Counter == 0); //not changed
        }

        [Fact]
        public void ChangeTrackingCollection()
        {
            try
            {
                ItemB objA = new ItemB();
                objA.Items.Add(new SubItem() { ItemB = new ItemB() { Id = 1 } });
                
                objA = objA.AsTrackable();
                objA.Items.Add(new SubItem() { ItemB = new ItemB() { Id = 3 } });

                UpdateItem updateItem = new UpdateItem();

                CreateMapper().Map(objA, updateItem);

                Assert.True(updateItem.ItemsAdded.Count == 1);  //changed
                Assert.True(updateItem.ItemsAdded[0].ItemBId == 3);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        [Fact]
        public void IdToEntity()
        {
            UpdateItem updateItem = new UpdateItem();
            updateItem.CategoryId = 123;

            ItemA itemA = new ItemA();

            CreateMapper().Map(updateItem, itemA);
            
            Assert.True(itemA.Category.Id == 123);
        }

        [Fact]
        public void EntityToId()
        {
            ItemA entity = new ItemA();
            entity.Category = new CategoryA() { Id = 123 };
          
            UpdateItem updateItem = new UpdateItem();

            CreateMapper().Map(entity, updateItem);

            Assert.True(updateItem.CategoryId == 123);
        }

        [Fact]
        public void Collection()
        {
            try
            {
                ItemA objA = new ItemA() { IsActive = true };
                objA.Items.Add(new SubItem() { SortKey = 2 });

                ItemB objB = new ItemB();

                CreateMapper().Map(objA, objB);

                Assert.True(objB.Items.Count == 1);
                Assert.True(objB.Items[0].SortKey == 2);
                Assert.True(ReferenceEquals(objA.Items[0], objB.Items[0]) == false);
            }
            catch (Exception ex)
            {

                throw;
            }            
        }
    }
}
