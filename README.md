## Technologies

- Dotnet 6
- Blazor Server
- Console Aplication
- Dapper
- SQLite

## How to run

- Navigate to the Blazor Project (BurgerBuddy)
- Dotnet run --urls http://0.0.0.0:5037 (This is the port the console app is expecting)
- Navigate to your browser and open the web page of 127.0.0.1:5037
- Navigate to Console app (BurgerBuddyOrderSim)
- Dotnet run

## Assumptions I have made

- This system is not client facing, it is for staff
- The customer does not need to see the returned Order.
- Notification of the orders status is left to a seperate system, not this one.
- Styling was not important in this Project
- Error messages liek Late Order or Limited Smiley meals throws an exception, this is the sorry message
