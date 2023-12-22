using Firebase.Extensions;
using Firebase.Firestore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class CubeHeroOdysseyStore : MonoBehaviour
{
    private FirebaseFirestore _db;
    private CollectionReference _Users;
    private Task<QuerySnapshot> _storageSnapshot;

    List<User> users = new List<User>();
    User user;


    private void Awake()
    {
        _storageSnapshot = FirebaseFirestore.DefaultInstance.Collection("CubeHeroGame").GetSnapshotAsync();
        _db = FirebaseFirestore.DefaultInstance;
        _Users = _db.Collection("CubeHeroGame");

        //StartCoroutine(GetUsers(null));
        //user = new User("Noob One", 10);
        //CreateUser(user);
        //StartCoroutine(GetUsersList(null));
        //StartCoroutine(GetUser(user, null));
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            StartCoroutine(GetUsersList(null));
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            StartCoroutine(GetUser(user, null));
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            UpdateUser(user);
        }
    }

    private string RandomStringGenerator()
    {
        // Creating object of random class 
        System.Random rand = new System.Random();

        // Choosing the size of string 
        // Using Next() string 
        int stringlen = rand.Next(4, 10);
        int randValue;
        string str = "";
        char letter;
        for (int i = 0; i < stringlen; i++)
        {

            // Generating a random number. 
            randValue = rand.Next(0, 26);

            // Generating random character by converting 
            // the random number into character. 
            letter = Convert.ToChar(randValue + 65);

            // Appending the letter to string. 
            str = str + letter;
        }
        Console.Write("Random String:" + str);
        return str;
    }


    public IEnumerator GetUser(User user, Action<User> callback)
    {
        IList<User> userList = new List<User>();
        int userCount = 0;
        _storageSnapshot.ContinueWithOnMainThread(task =>
        {
            var collection = task.Result;
            if (collection.Count == 0)
                Debug.LogError("There are no users available in the collection 'Users'.");

            userCount = collection.Count;
            foreach (var userData in collection.Documents)
            {
                var userName = userData.ToDictionary()["Username"] as string;

                if (userName == user.Username)
                {
                    var id = userData.ToDictionary()["Id"] as string;
                    var score = userData.ToDictionary()["Score"];
                    Debug.Log("User: " + id + ", Username: " + userName + " has a score of " + score);

                    user = new User(userName, Convert.ToInt32(score));
                    break;
                }
            }

        });
        yield return new WaitUntil(() => userList.Count == userCount && userList.Count != 0);
        Debug.Log("Done Getting User: " + user.Id + ", Username: " + user.Username + " has a score of " + user.Score);
        callback(user);
    }

    public IEnumerator GetUsersList(Action<IList<User>> callback)
    {
        IList<User> userList = new List<User>();
        int userCount = 0;
        _storageSnapshot.ContinueWithOnMainThread(task =>
        {
            var collection = task.Result;
            if (collection.Count == 0)
                Debug.LogError("There are no users available in the collection 'Users'.");

            userCount = collection.Count;
            Debug.Log("Found " + userCount + " users!");

            foreach (var userData in collection.Documents)
            {
                var id = userData.ToDictionary()["Id"] as string;
                var userName = userData.ToDictionary()["Username"] as string;
                var score = userData.ToDictionary()["Score"];
                Debug.Log("User: " + id + ", Username: " + userName + " has a score of " + score);
            }

        });
        yield return new WaitUntil(() => userList.Count == userCount && userList.Count != 0);
        Debug.Log("Done getting " + userCount + " users!");
        callback(userList);
    }

    public Task CreateUser(User user)
    {
        _Users.AddAsync(user).ContinueWithOnMainThread(task =>
        {
            DocumentReference addedDocRef = task.Result;
            Dictionary<string, object> idData = new Dictionary<string, object>
            {
                { "Id", addedDocRef.Id },
                { "Username", user.Username },
                { "Score", user.Score },
            };
            addedDocRef.SetAsync(idData);
            Debug.Log(String.Format("Added document with ID: {0}.", addedDocRef.Id));
        });

        return null;
    }

    public Task UpdateUser(User user)
    {
        string newName = RandomStringGenerator();
        string id = user.Id.ToString();

        Dictionary<string, object> updates = new Dictionary<string, object>
        {
            { "Username",  newName },
            { "Score", user.Score },
        };

        _Users.Document(id).UpdateAsync(updates).ContinueWithOnMainThread(task =>
        {
            Debug.Log(String.Format("Updated User: {0} - Username: {1}, Score: {2}.", user.Id, user.Username, user.Score));
        });
        // You can also update a single field with: cityRef.UpdateAsync("Capital", false);

        return null;
    }

    public Task DeleteUser(User user)
    {
        _Users.AddAsync(user).ContinueWithOnMainThread(task =>
        {
            DocumentReference addedDocRef = task.Result;
            Debug.Log(String.Format("Added document with ID: {0}.", addedDocRef.Id));
        });

        return null;
    }
}



// Create user  model
[FirestoreData]
public class User
{
    [FirestoreProperty]
    public string Id { get; set; }
    [FirestoreProperty]
    public string Username { get; set; }
    [FirestoreProperty]
    public int Score { get; set; }

    public User() { }

    public User(
        string userName,
        int score)
    {
        Username = userName;
        Score = score;
    }
}

