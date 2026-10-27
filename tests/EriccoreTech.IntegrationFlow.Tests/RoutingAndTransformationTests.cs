using System.Text.Json;
using EriccoreTech.IntegrationFlow.Processing;
using EriccoreTech.IntegrationFlow.Routing;
using EriccoreTech.IntegrationFlow.Transformation;
using Xunit;

namespace EriccoreTech.IntegrationFlow.Tests {
    public sealed class RoutingAndTransformationTests {

        [Fact]
        public void DynamicRouterUsesFirstMatchAndHonoursUnregister() {
            var router = new DynamicRouter();
            router.Register("high", m => m.GetProperty("priority").GetString() == "high");
            router.Register("any", _ => true);
            var message = JsonSerializer.SerializeToElement(new { priority = "high" });

            Assert.Equal("high", router.Route(message));
            router.Unregister("high");
            Assert.Equal("any", router.Route(message));
            router.Unregister("any");
            Assert.Null(router.Route(message));
        }

        [Fact]
        public void RecipientListRouterReturnsMatchingRules() {
            var message = JsonSerializer.SerializeToElement(new { region = "eu" });
            var recipients = new RecipientListRouter().Route(message, new Dictionary<string, string> { ["region"] = "eu", ["tier"] = "gold" });
            Assert.Equal(["region"], recipients);
        }

        [Fact]
        public void RoutingSlipReturnsRemainingSteps() {
            Assert.Equal(["b", "c"], new RoutingSlip().Next(["a", "b", "c"], 1));
        }

        [Fact]
        public void NormalizerMapsAliasesToCanonicalNames() {
            var input = JsonSerializer.SerializeToElement(new { mail = "a@b.c" });
            var output = new JsonNormalizer().Normalize(input, new Dictionary<string, string[]> { ["email"] = ["mail", "e-mail"] });
            Assert.Equal("a@b.c", output.GetProperty("email").GetString());
        }

        [Fact]
        public void EnricherKeepsExistingFieldsOverEnrichment() {
            var message = JsonSerializer.SerializeToElement(new { id = 1, tier = "gold" });
            var additions = new Dictionary<string, JsonElement> {
                ["tier"] = JsonSerializer.SerializeToElement("basic"),
                ["region"] = JsonSerializer.SerializeToElement("eu")
            };

            var output = new JsonContentEnricher().Enrich(message, additions);

            Assert.Equal("gold", output.GetProperty("tier").GetString());
            Assert.Equal("eu", output.GetProperty("region").GetString());
        }

        [Fact]
        public void TranslatorProjectsSelectedFields() {
            var input = JsonSerializer.SerializeToElement(new { first = "a", second = "b" });
            var output = new JsonMessageTranslator().Project(input, new Dictionary<string, string> { ["x"] = "second" });
            Assert.Equal("b", output.GetProperty("x").GetString());
            Assert.False(output.TryGetProperty("first", out _));
        }

        [Fact]
        public async Task ScatterGatherReturnsRepliesInRecipientOrder() {
            var request = JsonSerializer.SerializeToElement(1);
            var recipients = new List<Func<JsonElement, CancellationToken, Task<JsonElement>>> {
                async (r, _) => {
                    await Task.Delay(50);
                    return JsonSerializer.SerializeToElement("slow");
                },
                (r, _) => Task.FromResult(JsonSerializer.SerializeToElement("fast"))
            };

            var replies = await new ScatterGather().ExecuteAsync(request, recipients);

            Assert.Equal(["slow", "fast"], replies.Select(x => x.GetString()));
        }

        [Fact]
        public void ResequencerOrdersBySequence() {
            Assert.Equal([1, 2, 3], new Resequencer().Reorder([3, 1, 2], x => x));
        }

    }
}
