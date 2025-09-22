using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuruPR.Application.ModelConfigurations;

public class AzureOpenAIModelConfig
{
    public required string ModelName { get; init; }

    public required string DeploymentName { get; init; }

    public required string Endpoint { get; init; }

    public required string ApiVersion { get; init; }
}
