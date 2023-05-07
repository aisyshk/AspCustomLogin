namespace AspCustomLogin
{
    public class Startup
    {
        public IConfiguration Configuraton { get; }

        public Startup(IConfiguration configuration)
        {
            Configuraton = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            /// We'll be using this for our login handler.
            services.AddSession();
            services.AddControllersWithViews();

            /// Add your database context here with your unit of work, depending if you have one.
            // services.AddDbContext<>();
            // services.AddScoped<IUnitOfWork, UnitOfWork>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            // This is where we'll be using our session.
            app.UseSession();
            app.UseAuthorization();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=home}/{action=index}/{id?}");
            });

            /// This is where we'll be creating our database if it doesn't exist.
            /// You can also create your own database initializer class and call it here.
            //using (var ServiceScope = app.ApplicationServices.CreateScope())
            //{
            //    var context = ServiceScope.ServiceProvider.GetService<YourDbContext>();
            //    context.Database.EnsureCreated();
            //}   
        }
    }
}