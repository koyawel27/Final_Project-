using System;

public class CurrentUser
{
    // Define a property to store the UserType (Admin, Cashier, etc.)
    public string UserType { get; set; }

    // You can add other properties related to the current user
    public string UserName { get; set; }
    public int UserID { get; set; }

    // Constructor to initialize a new instance of CurrentUser
    public CurrentUser(string userType, string userName, int userId)
    {
        UserType = userType;
        UserName = userName;
        UserID = userId;
    }
}

