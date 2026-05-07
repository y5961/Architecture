API Endpoint Standards
•	Attributes: Use [ApiController] and [Route("api/[controller]")] on all controllers. Secure endpoints with [Authorize] unless public.
•	Thin Controllers: Controllers should delegate all business logic to services. Keep controller actions minimal.
•	Response Types: Use explicit return types (ActionResult<T>) and proper HTTP status codes.
•	Async: Controller actions should be async and await service calls.
•	Validation: Use model validation attributes and check ModelState.IsValid.
•	Documentation: Document endpoints with XML comments for Swagger.