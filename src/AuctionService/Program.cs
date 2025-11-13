using AuctionService.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddDbContext<AuctionDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultDatabase")));    

var app = builder.Build();
 

app.UseHttpsRedirection();


app.UseAuthorization();
app.MapControllers(); 

app.Run();

 
