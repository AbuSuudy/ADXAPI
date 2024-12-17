## Description
API that can set up an a ADX environment and return storm data. Uses role-based authentication from `JWT`.

## Set Up
-  Create ADX resources using this bicep [ADXAPI/Bicep/main.bicep](https://github.com/AbuSuudy/ADXAPI/blob/master/ADXAPI/Bicep/main.bicep)
-  Set up key used to generate `JWT` [ADXAPI/appsettings.json](https://github.com/AbuSuudy/ADXAPI/blob/master/ADXAPI/appsettings.json)
-  Call `GET /api/ADX/SetupStormEventTable` to generate table and ingest storm data. This done via batch process to may take few mins to retrieve from the table.

## Endpoints

## `GET /api/Auth/Login`

Returns `JWT` if the correct email and password is given that expires in 1 hour. 


| Query Parameters|
| ----------------| 
| Email           |
| Password        |

Example in memory users to test

| Email             | password  | ADXUser  |
|-------------------|-----------|-----------|
| `Test@gmail.com`  | `test`    |   `true`  |
| `Test2@gmail.com`  | `test`   |  `false`  |


## ADX Authorisation 
ADX Authorisation is only accessible if your `JWT` claim role is set to a `ADXUser`

```c#
 [Authorize (Roles= "ADXUser")]
 public class ADXController : ControllerBase
```

## `GET /api/ADX/SetupStormEventTable`

Will create tables if it doesn't exist and ingest storm event data into the table.

## `GET /api/ADX/GetStormData`

Return storm event data based on the kusto query 

```kusto
StormEvents
  | where EventType == 'Heavy Rain'
  | extend TotalDamage = DamageProperty + DamageCrops
  | summarize DailyDamage=sum(TotalDamage) by State, bin(StartTime, 1d)
  | where DailyDamage > 1000000
  | order by DailyDamage desc"
```

`Response`
``` json
[
  {
    "dateTime": "2007-12-03T00:00:00Z",
    "state": "WASHINGTON",
    "damageCost": 36020000
  },
  {
    "dateTime": "2007-12-02T00:00:00Z",
    "state": "WASHINGTON",
    "damageCost": 33000000
  },
  {
    "dateTime": "2007-06-28T00:00:00Z",
    "state": "MISSOURI",
    "damageCost": 8150000
  }
]
```

## Testing 
> [!WARNING]  
> As expected integration test will fail when azure resouce has been removed to save money. ADX resource could be re-created using this bicep [template](https://github.com/AbuSuudy/ADXAPI/blob/master/ADXAPI/Bicep/main.bicep)

## Integration Test
- Check if correct status code is returned from the API when different claims role types are used and if token is present or not.

## Unit Test
- Check if `JWT` generated by my service could be read by a handler, expire date is set to an the expected range and if correct audience is set.
- Mocking the `adx` and see if my service response has expected result

