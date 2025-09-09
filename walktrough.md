# Demo Walktrough

## Alba Setup

> Show `SystemUnderTest.cs/BasicHost()`

- Setup Test Project for Alba: Documentation
- `Program` refers to your app startup (F12 to show)

> Show `Basics.cs` and run tests

- Run a scenario
- Check the result
- Other HTTP Methods

## Authentication

> Run `Authentication.cs/Users_Me_Authorized()`

- Fails with 401

> Show and switch to `SystemUnderTest.cs/WithAuthenticationStub()`

- `JwtSecurityStub`: replaces all configuration in `AddAuthentication` and
  adds a valid JWT token to every request.

> Run tests again

- Authorized and show how request can be manipulated (remove headers)
- Show how other claims can be added to the token
- Show how Authorization policies can be tested

## Dependencies

> Run `Dependencies.cs/SubscribeNewsletter_HappyPath()`

- Fails with configuration error

> Show and switch to `SystemUnderTest.cs/WithMockedDependencies()`
> and run tests again

- Explain lifetime of mock and necessity of static
- Stress Importance of `Mock.Reset()`

## Database

> Run `Database.cs/Product_Does_Not_Exist()`

- Database is part of the System under tests.
- Too important to mock or use in-memory version

> Show `docker-compose.yml`

- Can be used in every environment: local of pipelines

> Show and switch to `SystemUnderTest.cs/WithDatrabase()`

- Explain overriding Configuration
- Explain building the database (also Arcus.Scripting.Sql)
- Explain rolling back the database (Respawn)

> Show the tests in `Database.cs`

- `Product_Exist` is not readable of maintainable
- `Product_Exist_Better` introduces helper methods to abstract yak shaving
- `Product_Exist_Best` introduces builder pattern
  - Maintainability
- `Product_Exist_Betterest` introduces Snapshot Testing
  - Contract Testing
  - Show `*.verified.txt`
  - Change tags in test and show diff viewer