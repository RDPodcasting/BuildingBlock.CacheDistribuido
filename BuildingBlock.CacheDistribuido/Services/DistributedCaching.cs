using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Serilog;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BuildingBlock.CacheDistribuido.Services
{
    public class DistributedCaching : IDistributedCaching
    {
        private readonly DistributedCacheEntryOptions Options;
        private readonly Logger Logger;
        private readonly IDistributedCache Cache;
        private TimeSpan TimeExpiredCached { get; set; }
        private string KeyIdentificadora { get; set; }

        public DistributedCaching(IDistributedCache cache, IConfiguration configuration)
        {
            Logger = new LoggerConfiguration()
               .WriteTo.Console()
               .CreateLogger();

            Logger.Information("Carregando configurações...");

            Cache = cache;
            KeyIdentificadora = configuration["REDIS_INSTANCE_NAME"];
            TimeExpiredCached = TimeSpan.FromSeconds(int.Parse(configuration["REDIS_TIME_EXPIRED_CACHED"]));
            Options = new DistributedCacheEntryOptions()
                      .SetSlidingExpiration(TimeExpiredCached);
            Logger.Information("Configurações carregadas com sucesso...");

            Logger.Information($"Nome da instância = {KeyIdentificadora}");
            Logger.Information($"Servidor = {configuration["REDIS_CONFIGURATION_URL"]}"); 
            Logger.Information($"Tempo de expiração = {TimeExpiredCached}");
        }
        public object GetCache(int Contador)
        {
            Object FromCache;
            try
            {
                FromCache = JsonConvert.DeserializeObject<Object>
                       (
                           Cache.GetString($"{KeyIdentificadora}{Contador}")
                       );

                Logger.Information($"Get {KeyIdentificadora}{Contador}");
            }
            catch (Exception ex)
            {
                Logger.Error($"Exceção: {ex.GetType().FullName} | " +
                            $"Mensagem: {ex.Message}");

                throw new ArgumentException($"Exceção: {ex.GetType().FullName} | " +
                               $"Mensagem: {ex.Message}");
            }
            return FromCache;
        }
        public T GetList<T>() where T : class, new()
        {
            List<Object> Lista = new List<Object>();
            bool Parar = true;
            int Contador = 0;

            Logger.Information("Iniciando busca da lista no Redis...");
            do
            {
                try
                {
                    Object FromCache = JsonConvert.DeserializeObject<Object>
                        (
                            Cache.GetString($"{KeyIdentificadora}{Contador}")
                        );

                    Logger.Information($"Get {KeyIdentificadora}{Contador}");

                    Lista.Add(FromCache);

                }
                catch (ArgumentNullException)
                {
                    Logger.Warning($"Finalizada a busca dos dados no redis, total de registros encontrados: {Lista.Count}");
                    Parar = false;
                }
                catch (Exception ex)
                {
                    Logger.Error($"Exceção: {ex.GetType().FullName} | " +
                                $"Mensagem: {ex.Message}");

                    throw new ArgumentException($"Exceção: {ex.GetType().FullName} | " +
                                $"Mensagem: {ex.Message}");
                }
            } while (Parar);

            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(Lista));
        }
        public async Task InsertCache(object o)
        {
            try
            {
                var value = JsonConvert.SerializeObject(o);
                Logger.Information($"inserindo {KeyIdentificadora}: {value}");
                await Cache.SetStringAsync($"{KeyIdentificadora}", value);
                Logger.Information("inserção realizada com sucesso no Redis!");

            }
            catch (Exception ex)
            {
                Logger.Error($"Exceção: {ex.GetType().FullName} | " +
                            $"Mensagem: {ex.Message}");

                throw new ArgumentException($"Exceção: {ex.GetType().FullName} | " +
                               $"Mensagem: {ex.Message}");
            }
        }
        public async Task InsertList(object[] listCache)
        {
            try
            {
                Logger.Information("Iniciando inserção de lista no Redis...");
                int contador = 0;

                foreach(Object o in listCache)
                {
                    var value = JsonConvert.SerializeObject(o);
                    Logger.Information($"insert {KeyIdentificadora}{contador}: {value}");
                    await Cache.SetStringAsync($"{KeyIdentificadora}{contador}",  value);
                    contador++;
                }
                Logger.Information("Fim da inserção de lista no Redis!");
            }
            catch(Exception ex)
            {
                Logger.Error($"Exceção: {ex.GetType().FullName} | " +
                             $"Mensagem: {ex.Message}");

                throw new ArgumentException($"Exceção: {ex.GetType().FullName} | " +
                               $"Mensagem: {ex.Message}");
            }
        }
        public async void NewCachedTimeExpered()
        {
            Logger.Information("New Cached Time Expired");

            await Cache.SetAsync($"{KeyIdentificadora}cachedTimeUTC",
            Encoding.UTF8.GetBytes(DateTime.UtcNow.ToString()), Options);

            Logger.Information($"New Cached Time: {await Cache.GetAsync($"{KeyIdentificadora}cachedTimeUTC")}" +
                               $" - Cached Time Expired: {TimeExpiredCached}");
        }
        public async Task<string> ValidationCachedTimeExpires()
        {
            Logger.Information("Recuperando o tempo de expiração do cache.");
            var encodedCachedTimeUTC = await Cache.GetAsync($"{KeyIdentificadora}cachedTimeUTC");

            var retorno = encodedCachedTimeUTC == null ? 
                "Cached Time Expired" : Encoding.UTF8.GetString(encodedCachedTimeUTC);

            Logger.Information($"Status de expiração do cache: {retorno}");
            return retorno;
        }
    }
}
