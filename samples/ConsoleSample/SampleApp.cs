using Ingenium.Data;
using Ingenium.Hosting;
using Ingenium.Storage;
using Ingenium.Tenants;

using Microsoft.Extensions.Logging;

namespace ConsoleSample;

public class SampleApp : App
{
	readonly ISqlFactory _sqlFactory;
	readonly IStorageFactory _storageFactory;

	public SampleApp(
		IAppServices services, 
		ISqlFactory sqlFactory,
		IStorageFactory storageFactory)
		: base(services)
	{
		_sqlFactory = sqlFactory;
		_storageFactory = storageFactory;
	}

	protected override async ValueTask RunAsync(CancellationToken cancellationToken)
	{
		//Logger.LogInformation("Example application.");

		//using var sql = _sqlFactory.CreateSqlContext();

		//int value = await sql.ReadScalarAsync<int>("select 1");

		//Logger.LogInformation($"Value read from database: {value}");

		var storage = _storageFactory.CreateStorage(WellKnownStorageProfiles.Temp);
		var storage2 = _storageFactory.CreateStorage(WellKnownStorageProfiles.Local, TenantId.Default);

		using var memory = new MemoryStream();
		using var writer = new StreamWriter(memory);
		await writer.WriteAsync("Hello There");
		await writer.FlushAsync();

		memory.Position = 0;

		await storage.PutAsync("file.txt", memory, conflictAction: StoreConflictAction.Replace, tenantId: TenantId.Default, cancellationToken: cancellationToken);
		await storage2.PutAsync("file.txt", memory, conflictAction: StoreConflictAction.Replace, tenantId: TenantId.Default, cancellationToken: cancellationToken);
	}
}