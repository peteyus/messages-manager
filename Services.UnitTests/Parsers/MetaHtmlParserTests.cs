namespace Services.UnitTests.Parsers
{
    using Core.Interfaces;
    using Core.Models;
    using Core.Models.Equality;
    using Moq;
    using Services.Parsers;
    using System.Globalization;
    using System.Reflection;

    [TestClass]
    public class FacebookHtmlParserTests
    {
        protected IMessageParser classUnderTest = new Mock<IMessageParser>().Object;
        protected MetaHtmlParserConfiguration testconfiguration = new MetaHtmlParserConfiguration();

        [TestInitialize]
        public void TestInitialize()
        {
            this.classUnderTest = new FacebookHtmlParser();
            this.testconfiguration.MessageHeaderIdentifer = "_2lej";
            this.testconfiguration.SenderNodeIdentifier = "_2lek";
            this.testconfiguration.TimestampNodeIdentifier = "_2lem";
            this.testconfiguration.ContentNodeIdentifier = "_2let";
        }

        [TestClass]
        public class ReadMessagesTests : FacebookHtmlParserTests
        {
            [TestMethod]
            public void ReadsSimpleMessageSegment()
            {
                // Arrange
                var html = @"
<div role=""main"">
    <div class=""_2lej"">
        <div class=""_2lek"">Jimbob MessageSender</div>
        <div class=""_2let"">
            <div>
                <div></div>
                <div>Here's my message content.</div>
            </div>
        </div>
        <div class=""_2lem"">Aug 19, 2021, 2:49 PM</div>
    </div>
</div>";

                // Act
                var message = this.classUnderTest.ReadMessages(html, this.testconfiguration).Single();

                // Assert
                Assert.IsNotNull(message);
                Assert.AreEqual("Jimbob MessageSender", message.Sender?.DisplayName, "Wrong name for sender.");
                Assert.AreEqual("Here's my message content.", message.MessageText, "Wrong message text.");
                Assert.AreEqual(DateTime.Parse("2021-08-19 14:49:00"), message.Timestamp, "Times don't match expected value.");
            }

            [TestMethod]
            public void ReadsMessageSegmentWithWeirdEmptyDivs()
            {
                // Arrange
                var html = @"
<div role=""main"">
    <div class=""_2lej"">
        <div class=""_2lek"">Jimbob MessageSender</div>
        <div class=""_2let"">
            <div>
                <div />
                <div>Here's my message content.</div>
                <div></div>
                <div></div>
            </div>
        </div>
        <div class=""_2lem"">Aug 19, 2021, 2:49 PM</div>
    </div>
</div>";

                // Act
                var message = this.classUnderTest.ReadMessages(html, this.testconfiguration).Single();

                // Assert
                Assert.IsNotNull(message);
                Assert.AreEqual("Jimbob MessageSender", message.Sender?.DisplayName, "Wrong name for sender.");
                Assert.AreEqual("Here's my message content.", message.MessageText, "Wrong message text.");
                Assert.AreEqual(DateTime.Parse("2021-08-19 14:49:00"), message.Timestamp, "Times don't match expected value.");
            }

            [TestMethod]
            public void ReadsMessageSegmentWtihSingleImage()
            {
                // Arrange
                var html = @"
<div role=""main"">
    <div class=""_2lej"">
        <div class=""_2lek"">Jimbob MessageSender</div>
        <div class=""_2let"">
            <div>
                <div />
                <div></div>
                <div></div>
                <div></div>
                <div><a href=""SomePath""><img src=""SomePath"" /></a></div>
                <div></div>
            </div>
        </div>
        <div class=""_2lem"">Aug 19, 2021, 2:49 PM</div>
    </div>
</div>";

                // Act
                var message = this.classUnderTest.ReadMessages(html, this.testconfiguration).Single();

                // Assert
                Assert.IsNotNull(message);
                Assert.AreEqual("Jimbob MessageSender", message.Sender?.DisplayName, "Wrong name for sender.");
                Assert.IsTrue(message.Images.Any(img => img.ImageUrl == "SomePath"), "Should have added the image source to the URLs.");
                Assert.AreEqual(1, message.Images.Count, "Should only have identified a single image node.");
            }

            [TestMethod]
            public void ReadsMessageWithVideo()
            {
                // Arrange
                var html = @"
<div role=""main"">
    <div class=""_2lej"">
        <div class=""_2lek"">Jimbob MessageSender</div>
        <div class=""_2let"">
            <div>
                <div />
                <div></div>
                <div></div>
                <div></div>
                <div><video src=""SomePath""><a href=""SomePath"" /></video></div>
                <div></div>
            </div>
        </div>
        <div class=""_2lem"">Aug 19, 2021, 2:49 PM</div>
    </div>
</div>";

                // Act
                var message = this.classUnderTest.ReadMessages(html, this.testconfiguration).Single();

                // Assert
                Assert.IsNotNull(message);
                Assert.AreEqual("Jimbob MessageSender", message.Sender?.DisplayName, "Wrong name for sender.");
                Assert.IsTrue(message.Videos.Any(vid => vid.VideoUrl == "SomePath"), "Should have added the video source to the URLs.");
                Assert.AreEqual(1, message.Videos.Count, "Should only have identified a single image node.");
            }
            [TestMethod]
            public void ReadsMessageSegmentWtihTwoImages()
            {
                // Arrange
                var html = @"
<div role=""main"">
    <div class=""_2lej"">
        <div class=""_2lek"">Jimbob MessageSender</div>
        <div class=""_2let"">
            <div>
                <div />
                <div></div>
                <div></div>
                <div></div>
                <div><a href=""SomePath""><img src=""SomePath"" /></a></div>
                <div><a href=""SomeOtherPath""><img src=""SomeOtherPath"" /></a></div>
                <div></div>
            </div>
        </div>
        <div class=""_2lem"">Aug 19, 2021, 2:49 PM</div>
    </div>
</div>";

                // Act
                var message = this.classUnderTest.ReadMessages(html, this.testconfiguration).Single();

                // Assert
                Assert.IsNotNull(message);
                Assert.AreEqual("Jimbob MessageSender", message.Sender?.DisplayName, "Wrong name for sender.");
                Assert.IsTrue(message.Images.Any(img => img.ImageUrl == "SomePath"), "Should have added the image source to the URLs.");
                Assert.IsTrue(message.Images.Any(img => img.ImageUrl == "SomeOtherPath"), "Should have added the image source to the URLs.");
                Assert.AreEqual(2, message.Images.Count, "Should have found both images.");
            }

            [TestMethod]
            public void DoesNotAddInReactionText()
            {
                // Arrange
                var html = @"
<div role=""main"">
    <div class=""_2lej"">
        <div class=""_2lek"">Jimbob MessageSender</div>
        <div class=""_2let"">
            <div>
                <div />
                <div>Here's my message content.</div>
                <div></div>
                <div></div>
                <div>
                    <ul class=""_tqp"">
                        <li>👍Janebob MessageReceiver</li>
                    </ul>
                </div>
            </div>
        </div>
        <div class=""_2lem"">Aug 19, 2021, 2:49 PM</div>
    </div>
</div>";

                // Act
                var message = this.classUnderTest.ReadMessages(html, this.testconfiguration).Single();

                // Assert
                Assert.IsNotNull(message);
                Assert.AreEqual("Jimbob MessageSender", message.Sender?.DisplayName, "Wrong name for sender.");
                Assert.AreEqual("Here's my message content.", message.MessageText, "Wrong message text.");
            }

            [TestMethod]
            public void ParsesSingleReactionAsExpected()
            {
                // Arrange
                var html = @"
<div role=""main"">
    <div class=""_2lej"">
        <div class=""_2lek"">Jimbob MessageSender</div>
        <div class=""_2let"">
            <div>
                <div />
                <div></div>
                <div>Here's my message content.</div>
                <div></div>
                <div>
                    <ul class=""_tqp"">
                        <li>👍Janebob MessageReceiver</li>
                    </ul>
                </div>
            </div>
        </div>
        <div class=""_2lem"">Aug 19, 2021, 2:49 PM</div>
    </div>
</div>";

                // Act
                var message = this.classUnderTest.ReadMessages(html, this.testconfiguration).Single();

                // Assert
                Assert.IsNotNull(message);
                Assert.AreEqual("Jimbob MessageSender", message.Sender?.DisplayName, "Wrong name for sender.");
                Assert.AreEqual(1, message.Reactions.Count, "Wrong number of reaction.");
                Assert.AreEqual("👍", message.Reactions[0].Reaction, "Did not capture expected reaction.");
                Assert.IsNotNull(message.Reactions[0].Person, "Should have identified a person.");
                Assert.AreEqual("Janebob MessageReceiver", message.Reactions[0].Person?.DisplayName, "Did not parse out the person.");
            }

            [TestMethod]
            public void ParsesTwoReactionsAsExpected()
            {
                // Arrange
                var html = @"
<div role=""main"">
    <div class=""_2lej"">
        <div class=""_2lek"">Jimbob MessageSender</div>
        <div class=""_2let"">
            <div>
                <div />
                <div>Here's my message content.</div>
                <div></div>
                <div></div>
                <div>
                    <ul class=""_tqp"">
                        <li>👍Janebob MessageReceiver</li>
                        <li>😉Jimbob MessageSender</li>
                    </ul>
                </div>
            </div>
        </div>
        <div class=""_2lem"">Aug 19, 2021, 2:49 PM</div>
    </div>
</div>";

                // Act
                var message = this.classUnderTest.ReadMessages(html, this.testconfiguration).Single();

                // Assert
                Assert.IsNotNull(message);
                Assert.AreEqual("Jimbob MessageSender", message.Sender?.DisplayName, "Wrong name for sender.");
                Assert.AreEqual(2, message.Reactions.Count, "Wrong number of reaction.");
                Assert.AreEqual("👍", message.Reactions[0].Reaction, "Did not capture expected reaction.");
                Assert.AreEqual("Janebob MessageReceiver", message.Reactions[0].Person?.DisplayName, "Did not parse out the person.");
                Assert.AreEqual("😉", message.Reactions[1].Reaction, "Did not capture expected reaction.");
                Assert.AreEqual("Jimbob MessageSender", message.Reactions[1].Person?.DisplayName, "Did not parse out the person.");
            }

            [TestMethod]
            public void ParseseMultipleMessages()
            {
                // Arrange
                var html = @"
<div role=""main"">
    <div class=""_2lej"">
        <div class=""_2lek"">Jimbob MessageSender</div>
        <div class=""_2let"">
            <div>
                <div />
                <div></div>
                <div>Here's my message content.</div>
                <div></div>
                <div>
                    <ul class=""_tqp"">
                        <li>👍Janebob MessageReceiver</li>
                        <li>😉Jimbob MessageSender</li>
                    </ul>
                </div>
            </div>
        </div>
        <div class=""_2lem"">Aug 19, 2021, 2:49 PM</div>
    </div>
    <div class=""_2lej"">
        <div class=""_2lek"">Janebob MessageReceiver</div>
        <div class=""_2let"">
            <div>
                <div />
                <div></div>
                <div>Here's my reply.</div>
                <div></div>
                <div>
                    <ul class=""_tqp"">
                        <li>😉Jimbob MessageSender</li>
                        <li>👍Janebob MessageReceiver</li>
                    </ul>
                </div>
            </div>
        </div>
        <div class=""_2lem"">Aug 19, 2021, 2:50 PM</div>
    </div>
</div>";

                // Act
                var messages = this.classUnderTest.ReadMessages(html, this.testconfiguration);

                // Assert
                Assert.AreEqual(2, messages.Count(), "Should have detected two messages.");
                Assert.AreEqual("Jimbob MessageSender", messages.First().Sender?.DisplayName, "Wrong name for sender.");
                Assert.AreEqual(2, messages.First().Reactions.Count, "Wrong number of reaction.");
                Assert.AreEqual("👍", messages.First().Reactions[0].Reaction, "Did not capture expected reaction.");
                Assert.AreEqual("😉", messages.First().Reactions[1].Reaction, "Did not capture expected reaction.");

                Assert.AreEqual("Janebob MessageReceiver", messages.Last().Sender?.DisplayName, "Wrong name for second sender.");
                Assert.AreEqual(2, messages.Last().Reactions.Count, "Wrong number of reaction.");
                Assert.AreEqual("😉", messages.Last().Reactions[0].Reaction, "Did not capture expected reaction.");
                Assert.AreEqual("👍", messages.Last().Reactions[1].Reaction, "Did not capture expected reaction.");
            }

            [TestMethod]
            public void ParsesMultipleMessageWithMultipleImagesAndReactions()
            {
                // Arrange
                var html = @"
<div role=""main"">
    <div class=""_2lej"">
        <div class=""_2lek"">Jimbob MessageSender</div>
        <div class=""_2let"">
            <div>
                <div />
                <div>Here's my message content.</div>
                <div></div>
                <div></div>
                <div><a href=""http://SomeUrl/""><img src=""SomePath"" /></a></div>
                <div><a href=""http://SomeOtherUrl/""><img src=""SomeOtherPath"" /></a></div>
                <div>
                    <ul class=""_tqp"">
                        <li>👍Janebob MessageReceiver</li>
                        <li>😉Jimbob MessageSender</li>
                    </ul>
                </div>
            </div>
        </div>
        <div class=""_2lem"">Aug 19, 2021, 2:49 PM</div>
    </div>
    <div class=""_2lej"">
        <div class=""_2lek"">Janebob MessageReceiver</div>
        <div class=""_2let"">
            <div>
                <div />
                <div>Here's my reply.</div>
                <div></div>
                <div></div>
                <div><a href=""http://SomeOtherUrl/""><img src=""SomeOtherPath"" /></a></div>
                <div><a href=""http://SomeUrl/""><img src=""SomePath"" /></a></div>
                <div>
                    <ul class=""_tqp"">
                        <li>😉Jimbob MessageSender</li>
                        <li>👍Janebob MessageReceiver</li>
                    </ul>
                </div>
            </div>
        </div>
        <div class=""_2lem"">Aug 19, 2021, 2:50 PM</div>
    </div>
</div>";

                // Act
                var messages = this.classUnderTest.ReadMessages(html, this.testconfiguration);

                // Assert
                Assert.AreEqual(2, messages.Count(), "Should have detected two messages.");
                Assert.AreEqual("Jimbob MessageSender", messages.First().Sender?.DisplayName, "Wrong name for sender.");
                Assert.AreEqual(2, messages.First().Reactions.Count, "Wrong number of reaction.");
                Assert.AreEqual("👍", messages.First().Reactions[0].Reaction, "Did not capture expected reaction.");
                Assert.AreEqual("😉", messages.First().Reactions[1].Reaction, "Did not capture expected reaction.");
                Assert.AreEqual(2, messages.First().Images.Count, "Wrong number of images");
                Assert.IsTrue(messages.First().Images[0]?.ImageUrl?.Equals("SomePath"), "Should have added the image source to the URLs.");
                Assert.IsTrue(messages.First().Images[1]?.ImageUrl?.Equals("SomeOtherPath"), "Should have added the image source to the URLs.");
                Assert.AreEqual("Here's my message content.", messages.First().MessageText, "Wrong message text.");

                Assert.AreEqual("Janebob MessageReceiver", messages.Last().Sender?.DisplayName, "Wrong name for second sender.");
                Assert.AreEqual(2, messages.Last().Reactions.Count, "Wrong number of reaction.");
                Assert.AreEqual("😉", messages.Last().Reactions[0].Reaction, "Did not capture expected reaction.");
                Assert.AreEqual("👍", messages.Last().Reactions[1].Reaction, "Did not capture expected reaction.");
                Assert.AreEqual(2, messages.Last().Images.Count, "Wrong number of images");
                Assert.IsTrue(messages.Last().Images[0]?.ImageUrl?.Equals("SomeOtherPath"), "Should have added the image source to the URLs.");
                Assert.IsTrue(messages.Last().Images[1]?.ImageUrl?.Equals("SomePath"), "Should have added the image source to the URLs.");
                Assert.AreEqual("Here's my reply.", messages.Last().MessageText, "Wrong message text.");
            }

            [TestMethod]
            public void IgnoresDivTagsThatDoNotContainMessages()
            {
                // Arrange
                var html = @"
<div role=""main"">
    <div></div>
    <div></div>
    <div class=""_2lej"">
        <div class=""_2lek"">Jimbob MessageSender</div>
        <div class=""_2let"">
            <div>
                <div />
                <div>Here's my message content.</div>
                <div></div>
                <div></div>
            </div>
        </div>
        <div class=""_2lem"">Aug 19, 2021, 2:49 PM</div>
    </div>
</div>";

                // Act
                var message = this.classUnderTest.ReadMessages(html, this.testconfiguration).Single();

                // Assert
                Assert.IsNotNull(message);
                Assert.AreEqual("Jimbob MessageSender", message.Sender?.DisplayName, "Wrong name for sender.");
                Assert.AreEqual("Here's my message content.", message.MessageText, "Did not capture the message text.");
            }
        }
    }

    [TestClass]
    public class InstagramHtmlParserTests
    {
        protected IMessageParser classUnderTest = new Mock<IMessageParser>().Object;
        protected MetaHtmlParserConfiguration testconfiguration = new MetaHtmlParserConfiguration();

        [TestInitialize]
        public void TestInitialize()
        {
            this.classUnderTest = new InstagramHtmlParser();

            this.testconfiguration.MessageHeaderIdentifer = "_2lej";
            this.testconfiguration.SenderNodeIdentifier = "_2lek";
            this.testconfiguration.TimestampNodeIdentifier = "_2lem";
            this.testconfiguration.ContentNodeIdentifier = "_2let";
        }

        [TestClass]
        public class ReadMessagesFromFileTests : InstagramHtmlParserTests
        {
            // note to future me: Not really unit tests, more integration since we're dealing with real filesystem,
            // but can't really mock it out of the HTML parser so we'll treat it as a unit.
            [TestMethod]
            public void ReadsExpectedNumberOfMessagesFromExternalFile()
            {
                // Arrange
                var testDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                Assert.IsNotNull(testDirectory);
                var filePath = Path.Combine(testDirectory, "Parsers", "Instagram-html-sample.html");

                // Act
                var messages = this.classUnderTest.ReadMessagesFromFile(filePath, this.testconfiguration);

                // Assert
                Assert.IsNotNull(messages);
                Assert.AreEqual(11, messages.Count(), "Should have found 11 messages.");
            }

            [TestMethod]
            public void CheckMessageContentOfExternalMessages()
            {
                // Arrange
                var testDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                Assert.IsNotNull(testDirectory);
                var filePath = Path.Combine(testDirectory, "Parsers", "Instagram-html-sample.html");

                var expectedMessages = new List<Message>
                {
                    new Message
                    {
                        MessageText = "Yeah it is :)",
                        Sender = new Person { DisplayName = "Receiving Person" },
                        Timestamp = new DateTimeOffset(DateTime.Parse("Sep 20, 2019, 6:28 AM", CultureInfo.InvariantCulture)),
                        Source = "Instagram"
                    },
                    new Message
                    {
                        MessageText = "This is totally us.",
                        Sender = new Person { DisplayName = "Other Person" },
                        Timestamp = new DateTimeOffset(DateTime.Parse("Sep 19, 2019, 8:46 AM", CultureInfo.InvariantCulture)),
                        Source = "Instagram"
                    },
                    new Message
                    {
                        Sender = new Person { DisplayName = "Other Person" },
                        Timestamp = new DateTimeOffset(DateTime.Parse("Sep 19, 2019, 8:46 AM", CultureInfo.InvariantCulture)),
                        Share = new Share
                        {
                            ShareText = "🌙⭐️ #catanacomics",
                            OriginalContentOwner = "catanacomics",
                            Url = "https://www.instagram.com/p/B08xGZ7gI1U/"
                        },
                        Source = "Instagram"
                    },
                    new Message
                    {
                        Sender = new Person { DisplayName = "Other Person" },
                        Timestamp = new DateTimeOffset(DateTime.Parse("Jul 24, 2019, 1:27 PM", CultureInfo.InvariantCulture)),
                        Share = new Share
                        {
                            ShareText = "This 12-year-old makes teddy bears IV covers to make hospitals less scary for kids. They plan to deliver 500 teddy bear IV covers to hospitals across the country by this fall- free of charge 🐻",
                            OriginalContentOwner = "nowthisnews",
                            Url = "https://www.instagram.com/p/B0Qi2TenfaX/"
                        },
                        Source = "Instagram"
                    },
                    new Message
                    {
                        Sender = new Person { DisplayName = "Other Person" },
                        Timestamp = new DateTimeOffset(DateTime.Parse("Jun 3, 2019, 10:44 AM", CultureInfo.InvariantCulture)),
                        Share = new Share
                        {
                            ShareText = "A hulk, white canary, zoom, groot, and scarlet witch have been hidden somewhere here. You guys know the drill, first 10ish to find them will get a shoutout on my story, and the rest will be awarded with eternal respect. Comment when you find them and don’t spoil it for others, enjoy :) .\n.\n.\n.\n.\n.\n.\n.\n.\n.\n. \n#dcshows #cwshows #karadanvers #karazorel #melissabenoist #supergirl #oliverqueen #stephenamell #arrow #barryallen #grantgustin #theflash #saralance #caitylotz #whitecanary #legendsoftomorrow #supercrossover #felicitysmoak #johndiggle #raypalmer #theaqueen #olicity #caitlinsnow #ciscoramon #iriswest #jeffersonjackson",
                            OriginalContentOwner = "arrowsmultiverse",
                            Url = "https://www.instagram.com/p/BxNhkqAD6i4/"
                        },
                        Source = "Instagram"
                    },
                    new Message
                    {
                        MessageText = "Not that one! Lol. It's the memories one. But that ones good too!",
                        Sender = new Person { DisplayName = "Other Person" },
                        Timestamp = new DateTimeOffset(DateTime.Parse("May 8, 2019, 1:55 PM", CultureInfo.InvariantCulture)),
                        Source = "Instagram"
                    },
                    new Message
                    {
                        MessageText = "Lol that's super cute",
                        Sender = new Person { DisplayName = "Receiving Person" },
                        Timestamp = new DateTimeOffset(DateTime.Parse("May 8, 2019, 1:52 PM", CultureInfo.InvariantCulture)),
                        Source = "Instagram"
                    },
                    new Message
                    {
                        MessageText = "I want this!",
                        Sender = new Person { DisplayName = "Other Person" },
                        Timestamp = new DateTimeOffset(DateTime.Parse("May 8, 2019, 11:36 AM", CultureInfo.InvariantCulture)),
                        Source = "Instagram"
                    },
                    new Message
                    {
                        Sender = new Person { DisplayName = "Other Person" },
                        Timestamp = new DateTimeOffset(DateTime.Parse("May 8, 2019, 11:36 AM", CultureInfo.InvariantCulture)),
                        Share = new Share
                        {
                            ShareText = "Ready for Mother's day? We have some fun projects mom would love! Or even a gift certificate! Stop by today!",
                            OriginalContentOwner = "heartfeltwallhangings",
                            Url = "https://www.instagram.com/p/BxLHErGnJcN/"
                        },
                        Source = "Instagram"
                    },
                    new Message
                    {
                        Sender = new Person { DisplayName = "Other Person" },
                        Timestamp = new DateTimeOffset(DateTime.Parse("Mar 27, 2019, 7:24 PM", CultureInfo.InvariantCulture)),
                        Source = "Instagram"
                    },
                    new Message
                    {
                        Sender = new Person { DisplayName = "Other Person" },
                        Timestamp = new DateTimeOffset(DateTime.Parse("Mar 27, 2019, 7:22 PM", CultureInfo.InvariantCulture)),
                        Share = new Share
                        {
                            ShareText = "Our mightiest Super Heroes assemble in the @LEGOMarvelGame Collection, available now! Relive all three #LEGOMarvel games in this action-packed adventure. #ad",
                            OriginalContentOwner = "marvel",
                            Url = "https://www.instagram.com/p/Bvgz2g0Fknj/"
                        },
                        Source = "Instagram"
                    }
                };

                // Act
                var messages = this.classUnderTest.ReadMessagesFromFile(filePath, this.testconfiguration);

                // Assert
                Assert.IsNotNull(messages);
                foreach (var message in expectedMessages)
                {
                    Assert.IsTrue(messages.Contains(message, new MessageEqualityComparer()), $"Did not find message {message}");
                }
            }
        }

        [TestClass]
        public class ConfigureParsingAndReturnSampleTests : InstagramHtmlParserTests
        {
            [TestMethod]
            public void GeneratesConfigurationFromSampleFile()
            {
                // arrange
                var testDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                Assert.IsNotNull(testDirectory);
                var filePath = Path.Combine(testDirectory, "Parsers", "Instagram-html-sample.html");

                var expectedMessage = new Message
                {
                    MessageText = "Yeah it is :)",
                    Sender = new Person { DisplayName = "Receiving Person" },
                    Timestamp = new DateTimeOffset(DateTime.Parse("Sep 20, 2019, 6:28 AM", CultureInfo.InvariantCulture)),
                    Source = "Instagram"
                };

                // act
                var sample = this.classUnderTest.ConfigureParsingAndReturnSample(filePath);

                // assert
                var config = sample.ParserConfiguration as MetaHtmlParserConfiguration;
                Assert.IsTrue(new MessageEqualityComparer().Equals(expectedMessage, sample.SampleMessage), "The message should match the expected message.");
                Assert.IsNotNull(config);
                Assert.AreEqual("uiBoxWhite", config.MessageHeaderIdentifer, "Failed to configure parsing from file.");
            }
        }
    }
}
