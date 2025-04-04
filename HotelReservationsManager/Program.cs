using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using HotelReservationsManager.Data.Models;
using HotelReservationsManager.Data;
using HotelReservationsManager.Services.Contracts;
using HotelReservationsManager.Services;

namespace HotelReservations.Web
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.
			var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

			builder.Services.AddDbContext<ApplicationDbContext>(options =>
			{
				options.UseSqlServer(connectionString);
				options.UseLazyLoadingProxies();
			});

			builder.Services.AddDatabaseDeveloperPageExceptionFilter();

			builder.Services
				.AddDefaultIdentity<User>(options =>
				{
					options.SignIn.RequireConfirmedAccount = false;
					options.Password.RequireNonAlphanumeric = false;
					options.Password.RequireDigit = false;
					options.Password.RequireLowercase = false;
					options.Password.RequireUppercase = false;
					options.Password.RequiredLength = 4;
				})
				.AddRoles<IdentityRole>()
				.AddEntityFrameworkStores<ApplicationDbContext>();


			builder.Services.AddControllersWithViews();

			builder.Services.AddTransient<IUsersService, UsersService>();
			builder.Services.AddTransient<IClientsService, ClientsService>();
			builder.Services.AddTransient<IRoomsService, RoomsService>();
			builder.Services.AddTransient<IReservationsService, ReservationsService>();

			var app = builder.Build();

			if (app.Environment.IsDevelopment())
			{
				app.UseMigrationsEndPoint();
			}
			else
			{
				app.UseExceptionHandler("/Home/Error");
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseStaticFiles();

			app.UseRouting();

			app.UseAuthorization();

			app.MapControllerRoute(
				name: "default",
				pattern: "{controller=Home}/{action=Index}/{id?}");

			app.MapRazorPages();  
			app.UseEndpoints(endpoints =>
			{
				endpoints.MapGet("/debug", async context =>
				{
					await context.Response.WriteAsync("Server is running!");
				});
			});

			app.Run();
		}
	}
}
