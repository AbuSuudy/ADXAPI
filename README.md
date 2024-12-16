## Description
API that is can set up an a ADX environment and return storm data. Uses role-based authentication that it received from the `JWT` token.

## Endpoints

## Login
### `GET /api/Auth/Login`

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

## ADX

## Authorisation 
All endpoints are only accessible if your `JWT` claim role is set to a `ADXUser`

## `GET /api/ADX/SetupStormEventTable`

Will create tables if it doesn't exist and ingest storm event data into the table.

## `GET /api/ADX/GetStormData`

Return StormEvent data based on the kusto query 

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

### Integration Test
- Check if correct status code is returned from the API with different claims role types and if token is not present

## Unit Test
- Check if `JWT` generated by my service could be read by a handler and expire date is set to an the expected datetime.
- Mocking the `adx` and see if my service response has expected result
