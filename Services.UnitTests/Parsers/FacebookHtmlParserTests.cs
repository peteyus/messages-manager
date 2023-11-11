﻿namespace Services.UnitTests.Parsers
{
    using Core.Interfaces;
    using Moq;
    using Services.Parsers;
    using System.IO.Abstractions;

    [TestClass]
    public class FacebookHtmlParserTests
    {
        private IMessageParser classUnderTest = new Mock<IMessageParser>().Object;

        [TestInitialize]
        public void TestInitialize()
        {
            this.classUnderTest = new FacebookHtmlParser(new Mock<IFileSystem>().Object);
        }

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
            Assert.IsTrue(message.ImageUrls.Contains("SomePath"), "Should have added the image source to the URLs.");
            Assert.AreEqual(1, message.ImageUrls.Count, "Should only have identified a single image node.");
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
            Assert.IsTrue(message.ImageUrls.Contains("SomePath"), "Should have added the image source to the URLs.");
            Assert.IsTrue(message.ImageUrls.Contains("SomeOtherPath"), "Should have added the image source to the URLs.");
            Assert.AreEqual(2, message.ImageUrls.Count, "Should have found both images.");
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
    }
}
