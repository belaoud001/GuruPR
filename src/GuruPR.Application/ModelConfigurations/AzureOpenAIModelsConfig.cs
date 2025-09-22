using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuruPR.Application.ModelConfigurations;

public class AzureOpenAIModelsConfig
{
    public required string ApiKey { get; init; }

    public required List<AzureOpenAIModelConfig> AzureOpenAIModels { get; init; }
}
