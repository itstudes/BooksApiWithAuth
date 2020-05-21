# BooksApi with Auth

## Summary
This project expands on the [MongoDb API tutorial provided by Microsoft](https://docs.microsoft.com/en-us/aspnet/core/tutorials/first-mongo-app?view=aspnetcore-3.1&tabs=visual-studio#add-a-controller). The project adds user functionality through the data model ApiUser. It uses the ApiUser model to add JWT authentication. Roles are also specified on the ApiUser model, these are used with authorization to only make particular API routes accessible depending on the user's role. The entire project is supposed to be a simple take on authentication and authorization of ASP.NET Core Web APIs without having to use Azure Active Directory, Azure Active Directory B2C or IdentityServer4 (although those solutions are significantly more secure). Also no SQL database is used, only MongoDb.

## Project Dependencies
* ASP.NET Core 3.1
* A local instance of MongoDb

## Resources
Here is a collection of resources that were used in putting this together:
* [Create a web API with ASP.NET Core and MongoDB](https://docs.microsoft.com/en-us/aspnet/core/tutorials/first-mongo-app?view=aspnetcore-3.1&tabs=visual-studio#add-a-controller)
* [ASP.NET Core 3.1 - Simple API for Authentication, Registration and User Management](https://jasonwatmore.com/post/2019/10/14/aspnet-core-3-simple-api-for-authentication-registration-and-user-management#program-cs)
* [Overview of ASP.NET Core authentication](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/?view=aspnetcore-3.1)
* [Introduction to authorization in ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/security/authorization/introduction?view=aspnetcore-3.1)
* [Introduction to Identity on ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/identity?view=aspnetcore-3.1&tabs=visual-studio)

## API Descriptions

### 1. UsersController
This controller contains all the routes for user related API queries. The definitions and routes are shown below.

#### Register [No Authorization Required]

```csharp
// route: /api/users/register
[AllowAnonymous]
[HttpPost("register")]
public IActionResult Register([FromBody] NewApiUser newApiUser){..}
```

#### Authenticate [No Authorization Required]

```csharp
// route: /api/users/authenticate
[AllowAnonymous]
[HttpPost("authenticate")]
public IActionResult Authenticate([FromBody] LoginModel newLogin){..}
```

#### Get [LibraryTeam Authorization Required]
```csharp
// route: /api/users
[HttpGet]
[Authorize(Policy = "LibraryTeam")]
public IActionResult Get(string? id, string? username, string? email){..}
```

#### Search [LibraryTeam Authorization Required]
```csharp
// route: /api/users/search
[HttpGet("search")]
[Authorize(Policy = "LibraryTeam")]
public IActionResult Search(string? firstName, string? lastName, int? role){..}
```

#### Delete [LibraryAdmin Authorization Required]
```csharp
// route: /api/users
[HttpDelete]
[Authorize(Roles = "LibraryAdmin")]
public IActionResult Delete(string id){..}
```

### 2. BooksController
This controller contains all the routes for books related API queries. The definitions and routes are shown below.

#### GetAll [Basic Authorization Required]

```csharp
// route: /api/books
[HttpGet]
public ActionResult<List<Book>> Get(){..}
```

#### Get [Basic Authorization Required]

```csharp
// route: /api/books/{mongoDbId}
[HttpGet("{id:length(24)}", Name = "GetBook")]
public ActionResult<Book> Get(string id){..}
```

#### Create [LibraryTeam Authorization Required]

```csharp
// route: /api/books
[HttpPost]
[Authorize(Policy = "LibraryTeam")]
public ActionResult<Book> Create(Book book){..}
```

#### Update [LibraryTeam Authorization Required]

```csharp
// route: /api/books/{mongoDbId}
[HttpPut("{id:length(24)}")]
[Authorize(Policy = "LibraryTeam")]
public IActionResult Update(string id, Book bookIn){..}
```

#### Delete [LibraryAdmin Authorization Required]

```csharp
// route: /api/books/{mongoDbId}
[HttpDelete("{id:length(24)}")]
[Authorize(Roles = "LibraryAdmin")]
public IActionResult Delete(string id){..}
```
