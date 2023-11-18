namespace Services.UnitTests.Parsers
{
    using Core.Interfaces;
    using Core.Models;
    using Core.Models.Equality;
    using Moq;
    using Services.Parsers;
    using System.IO.Abstractions;
    using System.Reflection;

    [TestClass]
    public class InstagramJsonParserTests
    {
        protected IMessageParser _classUnderTest = new Mock<IMessageParser>().Object;

        [TestInitialize]
        public void TestInitialize()
        {
            this._classUnderTest = new InstagramJsonParser(new Mock<IFileSystem>().Object);
        }

        [TestClass]
        public class ReadMessagesTests : InstagramJsonParserTests
        {
            [TestMethod]
            public void ReadsSingleMessage()
            {
                // arrange
                var json = @"{
    ""participants"": [
        {
            ""name"": ""Person One""
        },
        {
            ""name"": ""Person Two""
        }
    ],
    ""messages"": [
        {
            ""sender_name"": ""Person One"",
            ""timestamp_ms"": 1700010673608,
            ""content"": ""But that would be funny if you included that one"",
            ""is_geoblocked_for_viewer"": false
        }
    ]
}";

                // act
                var messages = this._classUnderTest.ReadMessages(json);

                // assert
                Assert.IsNotNull(messages);
                Assert.AreEqual(1, messages.Count());
                var message = messages.Single();
                Assert.AreEqual("Person One", message.Sender?.DisplayName);
                Assert.AreEqual("But that would be funny if you included that one", message.MessageText);
                Assert.AreEqual(DateTime.Parse("Nov 14, 2023, 6:11:13.608 PM"), message.Timestamp.ToLocalTime());
            }

            [TestMethod]
            public void DoesNotIncludeReactionInMessageText()
            {
                // arrange
                var json = @"{
    ""participants"": [
        {
            ""name"": ""Person One""
        },
        {
            ""name"": ""Person Two""
        }
    ],
    ""messages"": [
        {
            ""sender_name"": ""Person One"",
            ""timestamp_ms"": 1700010673608,
            ""content"": ""But that would be funny if you included that one"",
            ""reactions"": [
                {
                    ""reaction"": ""\u00f0\u009f\u00a4\u00a3"",
                    ""actor"": ""Person Two""
                }
            ],
            ""is_geoblocked_for_viewer"": false
        }
    ]
}";

                // act
                var messages = this._classUnderTest.ReadMessages(json, null);

                // assert
                Assert.IsNotNull(messages);
                var message = messages.Single();
                Assert.IsFalse(message.MessageText?.Contains("🤣"), "Should not have included emoji in message text.");
            }

            [TestMethod]
            public void ReadsMessageWithSingleImage()
            {
                // arrange
                var json = @"{
    ""participants"": [
        {
            ""name"": ""Person One""
        },
        {
            ""name"": ""Person Two""
        }
    ],
    ""messages"": [
        {
            ""sender_name"": ""Person One"",
            ""timestamp_ms"": 1700010673608,
            ""photos"": [
                {
                  ""uri"": ""messages/inbox/personOne/photos/photo.jpg"",
                  ""creation_timestamp"": 1700010673608
                }
            ],
            ""is_geoblocked_for_viewer"": false
        }
    ]
}";

                // act
                var messages = this._classUnderTest.ReadMessages(json, null);

                // assert
                Assert.IsNotNull(messages);
                var message = messages.Single();
                var image = message.Images.Single();
                Assert.AreEqual("messages/inbox/personOne/photos/photo.jpg", image.ImageUrl);
                Assert.AreEqual(message.Timestamp, image.CreatedAt);
            }

            [TestMethod]
            public void ReadsMessageWithAudio()
            {
                // arrange
                var json = @"{
    ""participants"": [
        {
            ""name"": ""Person One""
        },
        {
            ""name"": ""Person Two""
        }
    ],
    ""messages"": [
        {
            ""sender_name"": ""Person One"",
            ""timestamp_ms"": 1700010673608,
            ""audio_files"": [
                {
                  ""uri"": ""messages/inbox/personOne/audio/audio.aac"",
                  ""creation_timestamp"": 1700010673608
                }
            ],
            ""is_geoblocked_for_viewer"": false
        }
    ]
}";

                // act
                var messages = this._classUnderTest.ReadMessages(json, null);

                // assert
                Assert.IsNotNull(messages);
                var message = messages.Single();
                var audio = message.Audio.Single();
                Assert.AreEqual("messages/inbox/personOne/audio/audio.aac", audio.FileUrl);
                Assert.AreEqual(message.Timestamp, audio.CreatedAt);
            }

            [TestMethod]
            public void ReadsMessageWithVideo()
            {
                // arrange
                var json = @"{
    ""participants"": [
        {
            ""name"": ""Person One""
        },
        {
            ""name"": ""Person Two""
        }
    ],
    ""messages"": [
        {
            ""sender_name"": ""Person One"",
            ""timestamp_ms"": 1700010673608,
            ""videos"": [
                {
                  ""uri"": ""messages/inbox/personOne/videos/video.mp4""
                }
            ],
            ""is_geoblocked_for_viewer"": false
        }
    ]
}";

                // act
                var messages = this._classUnderTest.ReadMessages(json, null);

                // assert
                Assert.IsNotNull(messages);
                var message = messages.Single();
                var video = message.Videos.Single();
                Assert.AreEqual("messages/inbox/personOne/videos/video.mp4", video.VideoUrl);
            }

            [TestMethod]
            public void ReadsMessageWithMultipleImages()
            {
                // arrange
                var json = @"{
    ""participants"": [
        {
            ""name"": ""Person One""
        },
        {
            ""name"": ""Person Two""
        }
    ],
    ""messages"": [
        {
            ""sender_name"": ""Person One"",
            ""timestamp_ms"": 1700010673608,
            ""photos"": [
                {
                  ""uri"": ""messages/inbox/personOne/photos/photoOne.jpg"",
                  ""creation_timestamp"": 1700010673608
                },
                {
                  ""uri"": ""messages/inbox/personOne/photos/photoTwo.jpg"",
                  ""creation_timestamp"": 1700010673608
                }
            ],
            ""is_geoblocked_for_viewer"": false
        }
    ]
}";

                // act
                var messages = this._classUnderTest.ReadMessages(json, null);

                // assert
                Assert.IsNotNull(messages);
                var message = messages.Single();
                Assert.AreEqual(2, message.Images.Count());
                Assert.AreEqual("messages/inbox/personOne/photos/photoOne.jpg", message.Images.First().ImageUrl);
                Assert.AreEqual(message.Timestamp, message.Images.First().CreatedAt);
                Assert.AreEqual("messages/inbox/personOne/photos/photoTwo.jpg", message.Images.Last().ImageUrl);
                Assert.AreEqual(message.Timestamp, message.Images.Last().CreatedAt);
            }

            [TestMethod]
            public void ReadsMessageWithSingleReaction()
            {
                // arrange
                var json = @"{
    ""participants"": [
        {
            ""name"": ""Person One""
        },
        {
            ""name"": ""Person Two""
        }
    ],
    ""messages"": [
        {
            ""sender_name"": ""Person One"",
            ""timestamp_ms"": 1700010673608,
            ""content"": ""But that would be funny if you included that one"",
            ""reactions"": [
                {
                    ""reaction"": ""\u00f0\u009f\u00a4\u00a3"",
                    ""actor"": ""Person Two""
                }
            ],
            ""is_geoblocked_for_viewer"": false
        }
    ]
}";

                // act
                var messages = this._classUnderTest.ReadMessages(json, null);

                // assert
                Assert.IsNotNull(messages);
                var message = messages.Single();
                Assert.AreEqual("🤣", message.Reactions.Single().Reaction);
                Assert.AreEqual("Person Two", message.Reactions.Single().Person?.DisplayName);
            }

            [TestMethod]
            public void ReadsMessageWithMultipleReactions()
            {
                // arrange
                var json = @"{
    ""participants"": [
        {
            ""name"": ""Person One""
        },
        {
            ""name"": ""Person Two""
        }
    ],
    ""messages"": [
        {
            ""sender_name"": ""Person One"",
            ""timestamp_ms"": 1700010673608,
            ""content"": ""But that would be funny if you included that one"",
            ""reactions"": [
                {
                    ""reaction"": ""\u00f0\u009f\u00a4\u00a3"",
                    ""actor"": ""Person Two""
                },
                {
                    ""reaction"": ""\u00e2\u009d\u00a4\u00ef\u00b8\u008f"",
                    ""actor"": ""Person One""
                }
            ],
            ""is_geoblocked_for_viewer"": false
        }
    ]
}";

                // act
                var messages = this._classUnderTest.ReadMessages(json, null);

                // assert
                Assert.IsNotNull(messages);
                var message = messages.Single();
                Assert.AreEqual(2, message.Reactions.Count());
                Assert.AreEqual("🤣", message.Reactions.First().Reaction);
                Assert.AreEqual("Person Two", message.Reactions.First().Person?.DisplayName);
                Assert.AreEqual("❤️", message.Reactions.Last().Reaction);
                Assert.AreEqual("Person One", message.Reactions.Last().Person?.DisplayName);
            }

            [TestMethod]
            public void ReadsMultipleMessagse()
            {
                // arrange
                var json = @"{
    ""participants"": [
        {
            ""name"": ""Person One""
        },
        {
            ""name"": ""Person Two""
        }
    ],
    ""messages"": [
        {
            ""sender_name"": ""Person One"",
            ""timestamp_ms"": 1700010673608,
            ""content"": ""That funny thing I wrote that time"",
            ""reactions"": [
                {
                    ""reaction"": ""\u00f0\u009f\u00a4\u00a3"",
                    ""actor"": ""Person Two""
                }
            ],
            ""is_geoblocked_for_viewer"": false
        },
        {
            ""sender_name"": ""Person Two"",
            ""timestamp_ms"": 1700010673000,
            ""content"": ""Some inane something or other"",
            ""is_geoblocked_for_viewer"": false
        }
    ]
}";

                // act
                var messages = this._classUnderTest.ReadMessages(json, null);

                // assert
                Assert.IsNotNull(messages);
                Assert.AreEqual(2, messages.Count());
                Assert.AreEqual("Person One", messages.First().Sender?.DisplayName);
                Assert.AreEqual("That funny thing I wrote that time", messages.First().MessageText);
                Assert.AreEqual(DateTime.Parse("Nov 14, 2023, 6:11:13.608 PM"), messages.First().Timestamp.ToLocalTime());

                Assert.AreEqual("Person Two", messages.Last().Sender?.DisplayName);
                Assert.AreEqual("Some inane something or other", messages.Last().MessageText);
                Assert.AreEqual(DateTime.Parse("Nov 14, 2023, 6:11:13.000 PM"), messages.Last().Timestamp.ToLocalTime());
            }

            [TestMethod]
            public void ReadsMessagesWithMixOfMessageMediaAndReactions()
            {
                // arrange
                var json = @"{
    ""participants"": [
        {
            ""name"": ""Person One""
        },
        {
            ""name"": ""Person Two""
        }
    ],
    ""messages"": [
        {
            ""sender_name"": ""Person One"",
            ""timestamp_ms"": 1700010673608,
            ""content"": ""That funny thing I wrote that time"",
            ""is_geoblocked_for_viewer"": false
        },
        {
            ""sender_name"": ""Person Two"",
            ""timestamp_ms"": 1700010673000,
            ""content"": ""Some inane something or other"",
            ""is_geoblocked_for_viewer"": false
        },
        {
            ""sender_name"": ""Person One"",
            ""timestamp_ms"": 1700010672500,
            ""videos"": [
                {
                  ""uri"": ""messages/inbox/personOne/videos/video.mp4""
                }
            ],
            ""reactions"": [
                {
                    ""reaction"": ""\u00f0\u009f\u00a4\u00a3"",
                    ""actor"": ""Person Two""
                }
            ],
            ""is_geoblocked_for_viewer"": false
        },
        {
            ""sender_name"": ""Person One"",
            ""timestamp_ms"": 1700010672000,
            ""audio_files"": [
                {
                  ""uri"": ""messages/inbox/personOne/audio/audio.aac"",
                  ""creation_timestamp"": 1700010672000
                }
            ],
            ""is_geoblocked_for_viewer"": false
        }
    ]
}";

                var expectedMessages = new Message[]
                {
                    new Message
                    {
                        Sender = new Person { DisplayName = "Person One" },
                        Timestamp = DateTimeOffset.FromUnixTimeMilliseconds(1700010673608),
                        MessageText = "That funny thing I wrote that time",
                        Source = "Instagram"
                    },
                    new Message
                    {
                        Sender = new Person { DisplayName = "Person Two" },
                        Timestamp = DateTimeOffset.FromUnixTimeMilliseconds(1700010673000),
                        MessageText = "Some inane something or other",
                        Source = "Instagram"
                    },
                    new Message
                    {
                        Sender = new Person { DisplayName = "Person One" },
                        Timestamp = DateTimeOffset.FromUnixTimeMilliseconds(1700010672500),
                        Videos = new[] { new Video { VideoUrl = "messages/inbox/personOne/videos/video.mp4" } },
                        Reactions = new[] { new MessageReaction { Person = new Person { DisplayName = "Person Two"}, Reaction = "🤣" } },
                        Source = "Instagram"
                    },
                    new Message
                    {
                        Sender = new Person { DisplayName = "Person One" },
                        Timestamp = DateTimeOffset.FromUnixTimeMilliseconds(1700010672000),
                        Audio = new[] { new Audio { FileUrl = "messages/inbox/personOne/audio/audio.aac", CreatedAt = DateTimeOffset.FromUnixTimeMilliseconds(1700010672000) } },
                        Source = "Instagram"
                    }
                };

                // act
                var messages = this._classUnderTest.ReadMessages(json, null);

                // assert
                Assert.IsNotNull(messages);
                Assert.AreEqual(4, messages.Count());
                foreach (var expectedMessage in expectedMessages)
                {
                    Assert.IsTrue(messages.Contains(expectedMessage, new MessageEqualityComparer()));
                }
            }
        }

        [TestClass]
        public class ReadMessagesFromFileTests : InstagramJsonParserTests
        {
            [TestMethod]
            public void ReadsExpectedMessagesFromFile()
            {
                // re-init with real filesystem
                this._classUnderTest = new InstagramJsonParser(new FileSystem());

                // arrange
                var testDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                Assert.IsNotNull(testDirectory);
                var filePath = Path.Combine(testDirectory, "Parsers", "instagram-json-sample.json");
                
                var expectedMessages = new Message[] {
                    new Message
                    {
                        Sender = new Person { DisplayName = "Person One" },
                        Timestamp = DateTimeOffset.FromUnixTimeMilliseconds(1700010673608),
                        MessageText = "That funny thing I wrote that time",
                        Source = "Instagram"
                    },
                    new Message
                    {
                        Sender = new Person { DisplayName = "Person Two" },
                        Timestamp = DateTimeOffset.FromUnixTimeMilliseconds(1700010673000),
                        MessageText = "Some inane something or other",
                        Source = "Instagram"
                    },
                    new Message
                    {
                        Sender = new Person { DisplayName = "Person One" },
                        Timestamp = DateTimeOffset.FromUnixTimeMilliseconds(1700010672500),
                        Videos = new[] { new Video { VideoUrl = "messages/inbox/personOne/videos/video.mp4" } },
                        Reactions = new[] { new MessageReaction { Person = new Person { DisplayName = "Person Two"}, Reaction = "🤣" } },
                        Source = "Instagram"
                    },
                    new Message
                    {
                        Sender = new Person { DisplayName = "Person One" },
                        Timestamp = DateTimeOffset.FromUnixTimeMilliseconds(1700010672000),
                        Audio = new[] { new Audio { FileUrl = "messages/inbox/personOne/audio/audio.aac", CreatedAt = DateTimeOffset.FromUnixTimeMilliseconds(1700010672000) } },
                        Source = "Instagram"
                    }
};

                // act
                var messages = this._classUnderTest.ReadMessagesFromFile(filePath, null);

                // assert
                Assert.IsNotNull(messages);
                Assert.AreEqual(4, messages.Count());
                foreach (var expectedMessage in expectedMessages)
                {
                    Assert.IsTrue(messages.Contains(expectedMessage, new MessageEqualityComparer()));
                }
            }
        }
    }
}
