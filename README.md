# CoreMapper
Simple object mapper focused for Data Transfer Objects (DTO) and Entity Framework Core usage.  

https://www.nuget.org/packages/CoreMapper  
https://www.nuget.org/packages/CoreMapper.AspNetCore

## How to use it

```csharp
//create Mapper
Mapper mapper = new Mapper();

//IEntitySelector entitySelector = new EntitySelector();

mapper.RegisterStrategy<SameNameAndTypeStrategy>();
mapper.RegisterStrategy<EntityToIdStrategry>();
mapper.RegisterStrategy(new IdToEntityStrategy(entitySelector));
mapper.RegisterStrategy(new AddedToCollectionStrategy(entitySelector));
mapper.RegisterStrategy(new RemovedToCollectionStrategy(entitySelector));
mapper.RegisterStrategy<CollectionToAddedStrategy>();
mapper.RegisterStrategy<CollectionToRemovedStrategy>();
mapper.RegisterInterceptor<ChangeTrackingInterceptor>();

DbCustomer dbCustomer = new DbCustomer();
UpdateCustomerDTO customerDto = new UpdateCustomerDTO();

mapper.Map(customerDto, dbCustomer);
```
