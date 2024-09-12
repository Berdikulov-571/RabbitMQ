using RabbitMq.Reciever.Services;

namespace RabbitMq.Reciever
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Register ProducerService for sending messages
            //builder.Services.AddSingleton<IProducerService, ProducerService>();

            // Register ConsumerService for receiving messages
            builder.Services.AddHostedService<ConsumerService>();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();

        }
    }
}
