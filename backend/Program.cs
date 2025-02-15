/*
 * GRIPE - An app for complaining about companies.
 * Backend
 */

public partial class Program
{
	public static void Main(string[] args)
	{
		Console.WriteLine("Starting Gripe api");
		var builder = WebApplication.CreateSlimBuilder(args);

		var corsName = "gripeFrontendAngular";
		builder.Services.AddCors(options =>
		{
			options.AddPolicy(name: corsName,
							policy =>
							{
								policy
									.WithOrigins("http://localhost:4200") // angular frontend
									.AllowAnyHeader()
									.AllowAnyMethod();
							});
		});

		var app = builder.Build();
		app.UseCors(corsName);

		// init endpoints
		app.MapGet("/", () => "Welcome to the Gripe backend API!");
		app.MapComplaintEndpoints();
		app.MapUserEndpoints();

		app.Run();
	}
}