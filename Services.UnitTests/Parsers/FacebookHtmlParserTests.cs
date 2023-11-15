namespace Services.UnitTests.Parsers
{
    using Core.Interfaces;
    using Moq;
    using Services.Parsers;
    using System.IO.Abstractions;
    using System.Reflection;

    [TestClass]
    public class FacebookHtmlParserTests
    {
        protected IMessageParser classUnderTest = new Mock<IMessageParser>().Object;

        [TestInitialize]
        public void TestInitialize()
        {
            this.classUnderTest = new FacebookHtmlParser();
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
                <div>Here's my message content.</div>
            </div>
        </div>
        <div class=""_2lem"">Aug 19, 2021, 2:49 PM</div>
    </div>
</div>";

                // Act
                var message = this.classUnderTest.ReadMessages(html).Single();

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
                <div></div>
                <div>Here's my message content.</div>
                <div></div>
            </div>
        </div>
        <div class=""_2lem"">Aug 19, 2021, 2:49 PM</div>
    </div>
</div>";

                // Act
                var message = this.classUnderTest.ReadMessages(html).Single();

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
                <div><a href=""SomePath""><img src=""SomePath"" /></a></div>
                <div></div>
            </div>
        </div>
        <div class=""_2lem"">Aug 19, 2021, 2:49 PM</div>
    </div>
</div>";

                // Act
                var message = this.classUnderTest.ReadMessages(html).Single();

                // Assert
                Assert.IsNotNull(message);
                Assert.AreEqual("Jimbob MessageSender", message.Sender?.DisplayName, "Wrong name for sender.");
                Assert.IsTrue(message.Images.Any(img => img.ImageUrl == "SomePath"), "Should have added the image source to the URLs.");
                Assert.AreEqual(1, message.Images.Count, "Should only have identified a single image node.");
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
                <div><a href=""SomePath""><img src=""SomePath"" /></a></div>
                <div><a href=""SomeOtherPath""><img src=""SomeOtherPath"" /></a></div>
                <div></div>
            </div>
        </div>
        <div class=""_2lem"">Aug 19, 2021, 2:49 PM</div>
    </div>
</div>";

                // Act
                var message = this.classUnderTest.ReadMessages(html).Single();

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
                var message = this.classUnderTest.ReadMessages(html).Single();

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
                var message = this.classUnderTest.ReadMessages(html).Single();

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
</div>";

                // Act
                var message = this.classUnderTest.ReadMessages(html).Single();

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
                var messages = this.classUnderTest.ReadMessages(html);

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
                <div></div>
                <div>Here's my message content.</div>
                <div><a href=""http://SomeUrl/""><img src=""SomePath"" /></a></div>
                <div><a href=""http://SomeOtherUrl/""><img src=""SomeOtherPath"" /></a></div>
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
                <div><a href=""http://SomeOtherUrl/""><img src=""SomeOtherPath"" /></a></div>
                <div><a href=""http://SomeUrl/""><img src=""SomePath"" /></a></div>
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
                var messages = this.classUnderTest.ReadMessages(html);

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
                <div></div>
                <div>Here's my message content.</div>
                <div></div>
            </div>
        </div>
        <div class=""_2lem"">Aug 19, 2021, 2:49 PM</div>
    </div>
</div>";

                // Act
                var message = this.classUnderTest.ReadMessages(html).Single();

                // Assert
                Assert.IsNotNull(message);
                Assert.AreEqual("Jimbob MessageSender", message.Sender?.DisplayName, "Wrong name for sender.");
            }
        }

        [TestClass]

        public class ReadMessagesFromFileTests : FacebookHtmlParserTests
        {
            // note to future me: Not really unit tests, more integration since we're dealing with real filesystem,
            // but can't really mock it out of the HTML parser so we'll treat it as a unit.
            [TestMethod]
            public void ReadsInExpectedMessagesFromSmallFile()
            {
                // Arrange
                var testDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                Assert.IsNotNull(testDirectory);
                var filePath = Path.Combine(testDirectory, "Parsers", "Instagram-html-sample.html");

                // Act
                var messages = this.classUnderTest.ReadMessagesFromFile(filePath);

                // Assert
                Assert.IsNotNull(messages);
                Assert.AreEqual(11, messages.Count(), "Should have found 11 messages.");
            }
        }
    }
}
