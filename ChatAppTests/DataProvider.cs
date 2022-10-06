using System.Globalization;
using ChatApp.DAL.Entities;
using Microsoft.AspNetCore.Identity;

namespace ChatAppTests;

public static class DataProvider
{
    public static List<ApplicationUser> Users { get; set; } = new()
    {
        new ApplicationUser
        {
            UserName = "username0", Id = "0",
            Email = "email0", PhoneNumber = "0000000000"
        },
        new ApplicationUser
        {
            UserName = "username1", Id = "1",
            Email = "email1", PhoneNumber = "0000000001"
        },
        new ApplicationUser
        {
            UserName = "username2", Id = "2",
            Email = "email2", PhoneNumber = "0000000002"
        },
        new ApplicationUser
        {
            UserName = "username3", Id = "3",
            Email = "email3", PhoneNumber = "0000000003"
        },
        new ApplicationUser
        {
            UserName = "username4", Id = "4",
            Email = "email4", PhoneNumber = "0000000004"
        }
    };

    public static List<Message> Messages { get; set; } = new()
    {
        new Message
        {
            Id = 3, ChatId = 0, UserId = "0",
            Text =
                "Message 0 from username0in username0 and username2 Personal Chat",
            DateTime = Convert.ToDateTime("01.10.2022 22:18:31",
                CultureInfo.CurrentCulture),
            ReplyTo = -1
        },
        new Message
        {
            Id = 4, ChatId = 0, UserId = "0",
            Text =
                "Message 1 from username0in username0 and username2 Personal Chat",
            DateTime = Convert.ToDateTime("03.10.2022 05:31:31",
                CultureInfo.CurrentCulture),
            ReplyTo = -1
        },
        new Message
        {
            Id = 5, ChatId = 0, UserId = "0",
            Text =
                "Message 2 from username0in username0 and username2 Personal Chat",
            DateTime = Convert.ToDateTime("01.10.2022 23:28:31",
                CultureInfo.CurrentCulture),
            ReplyTo = -1
        },
        new Message
        {
            Id = 6, ChatId = 0, UserId = "0",
            Text =
                "Message 3 from username0in username0 and username2 Personal Chat",
            DateTime = Convert.ToDateTime("30.09.2022 17:40:31",
                CultureInfo.CurrentCulture),
            ReplyTo = -1
        },
        new Message
        {
            Id = 7, ChatId = 0, UserId = "0",
            Text =
                "Message 4 from username0in username0 and username2 Personal Chat",
            DateTime = Convert.ToDateTime("03.10.2022 06:09:31",
                CultureInfo.CurrentCulture),
            ReplyTo = -1
        },
        new Message
        {
            Id = 11, ChatId = 8, UserId = "0",
            Text =
                "Message 0 from username0in username0 and username4 Personal Chat",
            DateTime = Convert.ToDateTime("03.10.2022 05:48:31",
                CultureInfo.CurrentCulture),
            ReplyTo = -1
        },
        new Message
        {
            Id = 12, ChatId = 8, UserId = "0",
            Text =
                "Message 1 from username0in username0 and username4 Personal Chat",
            DateTime = Convert.ToDateTime("04.10.2022 08:47:31",
                CultureInfo.CurrentCulture),
            ReplyTo = -1
        },
        new Message
        {
            Id = 13, ChatId = 8, UserId = "0",
            Text =
                "Message 2 from username0in username0 and username4 Personal Chat",
            DateTime = Convert.ToDateTime("01.10.2022 15:46:31",
                CultureInfo.CurrentCulture),
            ReplyTo = -1
        },
        new Message
        {
            Id = 14, ChatId = 8, UserId = "0",
            Text =
                "Message 3 from username0in username0 and username4 Personal Chat",
            DateTime = Convert.ToDateTime("03.10.2022 18:46:31",
                CultureInfo.CurrentCulture),
            ReplyTo = -1
        },
        new Message
        {
            Id = 15, ChatId = 8, UserId = "0",
            Text =
                "Message 4 from username0in username0 and username4 Personal Chat",
            DateTime = Convert.ToDateTime("01.10.2022 00:24:31",
                CultureInfo.CurrentCulture),
            ReplyTo = -1
        },
        new Message
        {
            Id = 19, ChatId = 16, UserId = "1",
            Text =
                "Message 0 from username1in username1 and username4 Personal Chat",
            DateTime = Convert.ToDateTime("03.10.2022 19:18:31",
                CultureInfo.CurrentCulture),
            ReplyTo = -1
        },
        new Message
        {
            Id = 20, ChatId = 16, UserId = "1",
            Text =
                "Message 1 from username1in username1 and username4 Personal Chat",
            DateTime = Convert.ToDateTime("01.10.2022 01:55:31",
                CultureInfo.CurrentCulture),
            ReplyTo = -1
        },
        new Message
        {
            Id = 21, ChatId = 16, UserId = "1",
            Text =
                "Message 2 from username1in username1 and username4 Personal Chat",
            DateTime = Convert.ToDateTime("03.10.2022 14:40:31",
                CultureInfo.CurrentCulture),
            ReplyTo = -1
        },
        new Message
        {
            Id = 22, ChatId = 16, UserId = "1",
            Text =
                "Message 3 from username1in username1 and username4 Personal Chat",
            DateTime = Convert.ToDateTime("03.10.2022 00:02:31",
                CultureInfo.CurrentCulture),
            ReplyTo = -1
        },
        new Message
        {
            Id = 23, ChatId = 16, UserId = "1",
            Text =
                "Message 4 from username1in username1 and username4 Personal Chat",
            DateTime = Convert.ToDateTime("30.09.2022 10:30:31",
                CultureInfo.CurrentCulture),
            ReplyTo = -1
        },
        new Message
        {
            Id = 27, ChatId = 24, UserId = "2",
            Text =
                "Message 0 from username2in username2 and username3 Personal Chat",
            DateTime = Convert.ToDateTime("30.09.2022 12:24:31",
                CultureInfo.CurrentCulture),
            ReplyTo = -1
        },
        new Message
        {
            Id = 28, ChatId = 24, UserId = "2",
            Text =
                "Message 1 from username2in username2 and username3 Personal Chat",
            DateTime = Convert.ToDateTime("02.10.2022 14:53:31",
                CultureInfo.CurrentCulture),
            ReplyTo = -1
        },
        new Message
        {
            Id = 29, ChatId = 24, UserId = "2",
            Text =
                "Message 2 from username2in username2 and username3 Personal Chat",
            DateTime = Convert.ToDateTime("04.10.2022 09:52:31",
                CultureInfo.CurrentCulture),
            ReplyTo = -1
        },
        new Message
        {
            Id = 30, ChatId = 24, UserId = "2",
            Text =
                "Message 3 from username2in username2 and username3 Personal Chat",
            DateTime = Convert.ToDateTime("30.09.2022 15:14:31",
                CultureInfo.CurrentCulture),
            ReplyTo = -1
        },
        new Message
        {
            Id = 31, ChatId = 24, UserId = "2",
            Text =
                "Message 4 from username2in username2 and username3 Personal Chat",
            DateTime = Convert.ToDateTime("03.10.2022 23:12:31",
                CultureInfo.CurrentCulture),
            ReplyTo = -1
        },
        new Message
        {
            Id = 35, ChatId = 32, UserId = "2",
            Text =
                "Message 0 from username2in username2 and username4 Personal Chat",
            DateTime = Convert.ToDateTime("03.10.2022 02:16:31",
                CultureInfo.CurrentCulture),
            ReplyTo = -1
        },
        new Message
        {
            Id = 36, ChatId = 32, UserId = "2",
            Text =
                "Message 1 from username2in username2 and username4 Personal Chat",
            DateTime = Convert.ToDateTime("03.10.2022 19:47:31",
                CultureInfo.CurrentCulture),
            ReplyTo = -1
        },
        new Message
        {
            Id = 37, ChatId = 32, UserId = "2",
            Text =
                "Message 2 from username2in username2 and username4 Personal Chat",
            DateTime = Convert.ToDateTime("01.10.2022 12:29:31",
                CultureInfo.CurrentCulture),
            ReplyTo = -1
        },
        new Message
        {
            Id = 38, ChatId = 32, UserId = "2",
            Text =
                "Message 3 from username2in username2 and username4 Personal Chat",
            DateTime = Convert.ToDateTime("04.10.2022 06:56:31",
                CultureInfo.CurrentCulture),
            ReplyTo = -1
        },
        new Message
        {
            Id = 39, ChatId = 32, UserId = "2",
            Text =
                "Message 4 from username2in username2 and username4 Personal Chat",
            DateTime = Convert.ToDateTime("01.10.2022 00:14:31",
                CultureInfo.CurrentCulture),
            ReplyTo = -1
        },
        new Message
        {
            Id = 0, ChatId = 40, UserId = "0",
            Text = "username0 message 0 in chat Group chat 0",
            DateTime = Convert.ToDateTime("03.10.2022 01:56:31",
                CultureInfo.CurrentCulture),
            ReplyTo = -1
        },
        new Message
        {
            Id = 0, ChatId = 42, UserId = "0",
            Text = "username0 message 0 in chat Group chat 1",
            DateTime = Convert.ToDateTime("04.10.2022 08:35:31",
                CultureInfo.CurrentCulture),
            ReplyTo = -1
        },
        new Message
        {
            Id = 0, ChatId = 42, UserId = "0",
            Text = "username0 message 1 in chat Group chat 1",
            DateTime = Convert.ToDateTime("02.10.2022 05:19:31",
                CultureInfo.CurrentCulture),
            ReplyTo = -1
        },
        new Message
        {
            Id = 0, ChatId = 42, UserId = "0",
            Text = "username0 message 2 in chat Group chat 1",
            DateTime = Convert.ToDateTime("01.10.2022 22:04:31",
                CultureInfo.CurrentCulture),
            ReplyTo = -1
        },
        new Message
        {
            Id = 0, ChatId = 42, UserId = "0",
            Text = "username0 message 0 in chat Group chat 1",
            DateTime = Convert.ToDateTime("01.10.2022 03:32:31",
                CultureInfo.CurrentCulture),
            ReplyTo = -1
        }
    };

    public static List<Chat> Chats { get; set; } = new()
    {
        new Chat
        {
            Id = 0, Name = "username0 and username2 Personal Chat",
            IsPersonal = true
        },
        new Chat
        {
            Id = 8, Name = "username0 and username4 Personal Chat",
            IsPersonal = true
        },
        new Chat
        {
            Id = 16, Name = "username1 and username4 Personal Chat",
            IsPersonal = true
        },
        new Chat
        {
            Id = 24, Name = "username2 and username3 Personal Chat",
            IsPersonal = true
        },
        new Chat
        {
            Id = 32, Name = "username2 and username4 Personal Chat",
            IsPersonal = true
        },
        new Chat
        {
            Id = 40, Name = "Group chat 0", IsPersonal = false
        },
        new Chat
        {
            Id = 42, Name = "Group chat 1", IsPersonal = false
        }
    };

    public static List<MemberChat> MembersChats { get; set; } = new()
    {
        new MemberChat {Id = 1, ChatId = 1, UserId = "0"},
        new MemberChat {Id = 2, ChatId = 2, UserId = "2"},
        new MemberChat {Id = 9, ChatId = 9, UserId = "0"},
        new MemberChat {Id = 10, ChatId = 10, UserId = "4"},
        new MemberChat {Id = 17, ChatId = 17, UserId = "1"},
        new MemberChat {Id = 18, ChatId = 18, UserId = "4"},
        new MemberChat {Id = 25, ChatId = 25, UserId = "2"},
        new MemberChat {Id = 26, ChatId = 26, UserId = "3"},
        new MemberChat {Id = 33, ChatId = 33, UserId = "2"},
        new MemberChat {Id = 34, ChatId = 34, UserId = "4"},
        new MemberChat {Id = 41, ChatId = 41, UserId = "0"},
        new MemberChat {Id = 43, ChatId = 43, UserId = "0"},
        new MemberChat {Id = 44, ChatId = 44, UserId = "4"},
        new MemberChat {Id = 45, ChatId = 45, UserId = "0"}
    };

    public static List<MessageDeletedForUser> MessagesDeletedForUsers 
        { get; set; } = new();

    private static int _currentId;
    
    public static void GenerateData()
    {
        var rand = new Random(42);
        const int usersCount = 5;
        const int groupChatsCount = 2;

        Users = new List<ApplicationUser>();
        Messages = new List<Message>();
        Chats = new List<Chat>();
        MembersChats = new List<MemberChat>();
        MessagesDeletedForUsers = new List<MessageDeletedForUser>();

        GenerateUsers(usersCount);
        GeneratePersonalChats(rand);
        GenerateGroupChats(rand, groupChatsCount);

        Console.Write("new List<ApplicationUser> {"
                      + string.Join(',', Users.Select(u =>
                          "new ApplicationUser {" + "UserName = \"" + u.UserName 
                          + "\", Id = \"" + u.Id + "\", Email = \"" + u.Email 
                          + "\", PhoneNumber = \"" + u.PhoneNumber + "\"}")) + "}");
        Console.Write("new List<Message> {"
                      + string.Join(',', Messages.Select(m =>
                          "new Message {" + "Id = " + m.Id
                          + ", ChatId = " + m.ChatId + ", UserId = \"" + m.UserId 
                          + "\", Text = \"" + m.Text 
                          + "\", DateTime = Convert.ToDateTime(\"" + m.DateTime 
                          + "\", CultureInfo.CurrentCulture), ReplyTo = " 
                          + m.ReplyTo + "}")) + "}");
        Console.Write("new List<Chat> {"
                      + string.Join(',', Chats.Select(c =>
                          "new Chat {" + "Id = " + c.Id
                          + ", Name = \"" + c.Name + "\", IsPersonal = " 
                          + c.IsPersonal + "}")) + "}");
        Console.Write("new List<MemberChat> {"
                      + string.Join(',', MembersChats.Select(mc =>
                          "new MemberChat {" + "Id = " + mc.Id
                          + ", ChatId = " + mc.Id + ", UserId = \"" + mc.UserId 
                          + "\"}")) + "}");
        Console.Write("new List<MessageDeletedForUser> {"
                      + string.Join(',', MessagesDeletedForUsers.Select(
                          mdfu =>
                          "new MessageDeletedForUser {" + "Id = " + mdfu.Id 
                          + ", MessageId = " + mdfu.Id + ", UserId = \""
                          + mdfu.UserId + "\"}")) + "}");
    }

    private static void GenerateUsers(int usersCount)
    {
        var hasher = new PasswordHasher<ApplicationUser>();
        for (var i = 0; i < usersCount; i++)
        {
            var user = new ApplicationUser
            {
                Id = i.ToString(),
                UserName = $"username{i}",
                Email = $"email{i}",
                PhoneNumber = $"{i:D10}"
            };
            user.PasswordHash = hasher.HashPassword(user, i.ToString());
            Users.Add(user);
        }
    }

    private static void GeneratePersonalChats(Random rand)
    {
        for (var i = 0; i < Users.Count; i++)
        {
            for (var j = i + 1; j < Users.Count; j++)
            {
                if (rand.Next(10) > 3)
                {
                    continue;
                }

                var id = _currentId++;
                var userName1 = Users[i].UserName;
                var userName2 = Users[j].UserName;
                if (string.CompareOrdinal(userName1, userName2) > 0)
                {
                    (userName1, userName2) = (userName2, userName1);
                }

                var chat = new Chat
                {
                    Id = id,
                    Name = $"{userName1} and {userName2} Personal Chat",
                    IsPersonal = true
                };
                Chats.Add(chat);
                
                MembersChats.Add(new MemberChat
                {
                    Id = _currentId++,
                    UserId = Users[i].Id,
                    ChatId = id
                });
                MembersChats.Add(new MemberChat
                {
                    Id = _currentId++,
                    UserId = Users[j].Id,
                    ChatId = id
                });

                GenerateMessages(rand, id, chat);
            }
        }
    }

    private static void GenerateMessages(Random rand, int id, Chat chat)
    {
        int i;
        int j;
        for (var k = 0; k < 5; k++)
        {
            var userIndex = rand.Next(1) == 0 ? i : j;
            var userId = Users[userIndex].Id;
            Messages.Add(new Message
            {
                UserId = userId,
                ChatId = id,
                DateTime = DateTime.Now - TimeSpan.FromMinutes(
                    rand.Next(7200)),
                Id = _currentId++,
                ReplyTo = -1,
                Text = $"Message {k} from {Users[userIndex].UserName}" +
                       $"in {chat.Name}"
            });
        }
    }

    private static void GenerateGroupChats(Random rand, int count)
    {
        for (var i = 0; i < count; i++)
        {
            var chat = new Chat
            {
                Id = _currentId++,
                Name = $"Group chat {i}",
                IsPersonal = false
            };
            Chats.Add(chat);
            
            var usersCount = rand.Next(Users.Count);
            for (var j = 0; j < usersCount; j++)
            {
                var userIndex = rand.Next(Users.Count);
                MembersChats.Add(new MemberChat
                {
                    Id = _currentId++,
                    ChatId = chat.Id,
                    UserId = Users[userIndex].Id
                });
                for (var k = 0; k < rand.Next(20); k++)
                {
                    Messages.Add(new Message
                    {
                        ChatId = chat.Id,
                        UserId = Users[userIndex].Id,
                        DateTime = DateTime.UtcNow - TimeSpan.FromMinutes(
                            rand.Next(7200)),
                        ReplyTo = -1,
                        Text = $"{Users[userIndex].UserName} message {k} " +
                               $"in chat {chat.Name}"
                    });
                }
            }
        }
    }
}