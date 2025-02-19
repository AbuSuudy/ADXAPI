﻿using ADXService.Entity;
using Kusto.Data;
using Kusto.Data.Common;
using Kusto.Data.Ingestion;
using Kusto.Data.Net.Client;
using Kusto.Ingest;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace ADXService
{
    public class ADXAccess : IADXAccess
    {
        private readonly string ingestUri;
        private readonly string tenantId;
        private KustoConnectionStringBuilder kustoConnectionStringBuilder;
        private readonly string databaseName = "adxdb";
        private readonly string tableName = "StormEvents";
        private readonly string tableMappingName = "StormEvents_CSV_Mapping";
        private readonly string blobPath = "https://kustosamples.blob.core.windows.net/samplefiles/StormEvents.csv";

        public ADXAccess(IConfiguration configuration)
        {
            ingestUri = configuration["KustoSettings:Kusto_Ingest_uri"];
            tenantId = configuration["KustoSettings:Tennant_Id"];
            kustoConnectionStringBuilder = new KustoConnectionStringBuilder(configuration["KustoSettings:Kusto_uri"]).WithAadAzCliAuthentication();

        }

        public async Task CreateTable()
        {
            using (var kustoClient = KustoClientFactory.CreateCslAdminProvider(kustoConnectionStringBuilder))
            {
                var command = CslCommandGenerator.GenerateTableCreateCommand(
                    tableName,
                    new[]
                    {
                        Tuple.Create("StartTime", "System.DateTime"),
                        Tuple.Create("EndTime", "System.DateTime"),
                        Tuple.Create("EpisodeId", "System.Int32"),
                        Tuple.Create("EventId", "System.Int32"),
                        Tuple.Create("State", "System.String"),
                        Tuple.Create("EventType", "System.String"),
                        Tuple.Create("InjuriesDirect", "System.Int32"),
                        Tuple.Create("InjuriesIndirect", "System.Int32"),
                        Tuple.Create("DeathsDirect", "System.Int32"),
                        Tuple.Create("DeathsIndirect", "System.Int32"),
                        Tuple.Create("DamageProperty", "System.Int32"),
                        Tuple.Create("DamageCrops", "System.Int32"),
                        Tuple.Create("Source", "System.String"),
                        Tuple.Create("BeginLocation", "System.String"),
                        Tuple.Create("EndLocation", "System.String"),
                        Tuple.Create("BeginLat", "System.Double"),
                        Tuple.Create("BeginLon", "System.Double"),
                        Tuple.Create("EndLat", "System.Double"),
                        Tuple.Create("EndLon", "System.Double"),
                        Tuple.Create("EpisodeNarrative", "System.String"),
                        Tuple.Create("EventNarrative", "System.String"),
                        Tuple.Create("StormSummary", "System.Object"),
                    }
                );
                await kustoClient.ExecuteControlCommandAsync(databaseName, command);
            }
        }

        public async Task IngestionMapping()
        {
            using (var kustoClient = KustoClientFactory.CreateCslAdminProvider(kustoConnectionStringBuilder))
            {
                var command = CslCommandGenerator.GenerateTableMappingCreateCommand(
                    IngestionMappingKind.Csv,
                    tableName,
                    tableMappingName,
                    new ColumnMapping[]
                    {
                        new() { ColumnName = "StartTime", Properties = new Dictionary<string, string> { { MappingConsts.Ordinal, "0" } } },
                        new() { ColumnName = "EndTime", Properties = new Dictionary<string, string> { { MappingConsts.Ordinal, "1" } } },
                        new() { ColumnName = "EpisodeId", Properties = new Dictionary<string, string> { { MappingConsts.Ordinal, "2" } } },
                        new() { ColumnName = "EventId", Properties = new Dictionary<string, string> { { MappingConsts.Ordinal, "3" } } },
                        new() { ColumnName = "State", Properties = new Dictionary<string, string> { { MappingConsts.Ordinal, "4" } } },
                        new() { ColumnName = "EventType", Properties = new Dictionary<string, string> { { MappingConsts.Ordinal, "5" } } },
                        new() { ColumnName = "InjuriesDirect", Properties = new Dictionary<string, string> { { MappingConsts.Ordinal, "6" } } },
                        new() { ColumnName = "InjuriesIndirect", Properties = new Dictionary<string, string> { { MappingConsts.Ordinal, "7" } } },
                        new() { ColumnName = "DeathsDirect", Properties = new Dictionary<string, string> { { MappingConsts.Ordinal, "8" } } },
                        new() { ColumnName = "DeathsIndirect", Properties = new Dictionary<string, string> { { MappingConsts.Ordinal, "9" } } },
                        new() { ColumnName = "DamageProperty", Properties = new Dictionary<string, string> { { MappingConsts.Ordinal, "10" } } },
                        new() { ColumnName = "DamageCrops", Properties = new Dictionary<string, string> { { MappingConsts.Ordinal, "11" } } },
                        new() { ColumnName = "Source", Properties = new Dictionary<string, string> { { MappingConsts.Ordinal, "12" } } },
                        new() { ColumnName = "BeginLocation", Properties = new Dictionary<string, string> { { MappingConsts.Ordinal, "13" } } },
                        new() { ColumnName = "EndLocation", Properties = new Dictionary<string, string> { { MappingConsts.Ordinal, "14" } } },
                        new() { ColumnName = "BeginLat", Properties = new Dictionary<string, string> { { MappingConsts.Ordinal, "15" } } },
                        new() { ColumnName = "BeginLon", Properties = new Dictionary<string, string> { { MappingConsts.Ordinal, "16" } } },
                        new() { ColumnName = "EndLat", Properties = new Dictionary<string, string> { { MappingConsts.Ordinal, "17" } } },
                        new() { ColumnName = "EndLon", Properties = new Dictionary<string, string> { { MappingConsts.Ordinal, "18" } } },
                        new() { ColumnName = "EpisodeNarrative", Properties = new Dictionary<string, string> { { MappingConsts.Ordinal, "19" } } },
                        new() { ColumnName = "EventNarrative", Properties = new Dictionary<string, string> { { MappingConsts.Ordinal, "20" } } },
                        new() { ColumnName = "StormSummary", Properties = new Dictionary<string, string> { { MappingConsts.Ordinal, "21" } } }
                    }
                );

                await kustoClient.ExecuteControlCommandAsync(databaseName, command);
            }
        }

        public async Task Batching()
        {
            using (var kustoClient = KustoClientFactory.CreateCslAdminProvider(kustoConnectionStringBuilder))
            {
                var command = CslCommandGenerator.GenerateTableAlterIngestionBatchingPolicyCommand(
                    databaseName,
                    tableName,
                    new IngestionBatchingPolicy(
                        maximumBatchingTimeSpan: TimeSpan.FromSeconds(10),
                        maximumNumberOfItems: 100,
                        maximumRawDataSizeMB: 1024
                    )
                );
                kustoClient.ExecuteControlCommand(command);
            }


            var ingestConnectionStringBuilder = new KustoConnectionStringBuilder(ingestUri).WithAadUserPromptAuthentication(tenantId);

            using (var ingestClient = KustoIngestFactory.CreateQueuedIngestClient(ingestConnectionStringBuilder))
            {
                var properties = new KustoQueuedIngestionProperties(databaseName, tableName)
                {
                    Format = DataSourceFormat.csv,
                    IngestionMapping = new IngestionMapping
                    {
                        IngestionMappingReference = tableMappingName,
                        IngestionMappingKind = IngestionMappingKind.Csv
                    },
                    IgnoreFirstRecord = true
                };

                await ingestClient.IngestFromStorageAsync(blobPath, properties);
            }

        }

        public bool CheckIfTableExist()
        {
            bool tableExist = false;

            using (var kustoClient = KustoClientFactory.CreateCslQueryProvider(kustoConnectionStringBuilder))
            {
                using (var response = kustoClient.ExecuteQuery(databaseName, ".show tables", null))
                {
                    int tableNameResult = response.GetOrdinal("TableName");

                    while (response.Read())
                    {
                        if (response.GetString(tableNameResult) == tableName)
                        {
                            tableExist = true;
                            break;
                        }
                    }
                }
            }

            return tableExist;
        }

        public List<StormData> StormEventsData()
        {
            List<StormData> stormDataList = new List<StormData>();

            using (var kustoClient = KustoClientFactory.CreateCslQueryProvider(kustoConnectionStringBuilder))
            {

                string query = @"StormEvents
                        | where EventType == 'Heavy Rain'
                        | extend TotalDamage = DamageProperty + DamageCrops
                        | summarize DailyDamage=sum(TotalDamage) by State, bin(StartTime, 1d)
                        | where DailyDamage > 1000000
                        | order by DailyDamage desc";

                using (IDataReader response = kustoClient.ExecuteQuery(databaseName, query, null))
                {
                    //GetOrdinal returns index based on the name
                    int columnNoStartTime = response.GetOrdinal("StartTime");
                    int columnNoState = response.GetOrdinal("State");
                    int columnNoDailyDamage = response.GetOrdinal("DailyDamage");

                    while (response.Read())
                    {
                        stormDataList.Add(new StormData
                        {
                            DateTime = response.GetDateTime(columnNoStartTime),
                            State = response.GetString(columnNoState),
                            DamageCost = response.GetInt64(columnNoDailyDamage)
                        });
                    }
                }
            }

            return stormDataList;
        }

        public long RowCount()
        {
            using (ICslQueryProvider cslQueryProvider = KustoClientFactory.CreateCslQueryProvider(kustoConnectionStringBuilder))
            {
                var results = cslQueryProvider.ExecuteQuery<long>(databaseName, $"{tableName} | count");

                return results.SingleOrDefault();
            };
        }

        public Dashboard DALDashboard(Dashboard dashboard)
        {
            using (var kustoClient = KustoClientFactory.CreateCslQueryProvider(kustoConnectionStringBuilder))
            {

                string query = @"StormEvents
                                  | where EventType == 'Heavy Rain'
                                  | extend TotalDamage = DamageProperty + DamageCrops
                                  | summarize DailyDamage=sum(TotalDamage) by State, bin(StartTime, 1d), BeginLon, BeginLat
                                  | order by DailyDamage desc
                                  | take 20";

                using (IDataReader response = kustoClient.ExecuteQuery(databaseName, query, null))
                {
                    //GetOrdinal returns index based on the name
                    int columnNoStartTime = response.GetOrdinal("StartTime");
                    int columnNoState = response.GetOrdinal("State");
                    int columnNoDailyDamage = response.GetOrdinal("DailyDamage");
                    int columnNoBeginLong = response.GetOrdinal("BeginLon");
                    int columnNoBeginLat = response.GetOrdinal("BeginLat");

                    dashboard.Data = new List<StateData>();
                    while (response.Read())
                    {
                        dashboard.Data.Add(new StateData
                        {
                            State = response.GetString(columnNoState),
                            DailyDamage = response.GetInt64(columnNoDailyDamage),
                            Lat = response.GetDouble(columnNoBeginLat),
                            Long = response.GetDouble(columnNoBeginLong),

                        });

                        dashboard.ChartData.Add(new ChartData
                        {
                            Data = new List<long>()
                            {
                               response.GetInt64(columnNoDailyDamage)
                            },
                            Name = response.GetString(columnNoState)
                        });
                    }
                }
            }

            return dashboard;
        }

        public Dashboard StormEventsDashboard()
        {
            Dashboard dashboard = new Dashboard
            {
                Data = new List<StateData>(),
                ChartData = new List<ChartData>()
            };

            DALDashboard(dashboard);

            dashboard.Average = dashboard.Data.Average(x => x.DailyDamage);

            //order once
            var list = dashboard.Data.OrderBy(x => x.DailyDamage).ToList();

            dashboard.Minimum = list.Select(x => new Minimum
            {
                State = x.State,
                DamageCost = x.DailyDamage,

            }).FirstOrDefault();

            dashboard.Maximum = list.Select(x => new Maximum
            {
                State = x.State,
                DamageCost = x.DailyDamage,

            }).LastOrDefault();

            return dashboard;
        }
    }
}