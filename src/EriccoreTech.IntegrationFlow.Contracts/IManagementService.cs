using EriccoreTech.IntegrationFlow.Core;

namespace EriccoreTech.IntegrationFlow.Contracts {
    public interface IManagementService {

        ListChannelsResponse ListChannels(ListChannelsRequest request);

        PauseChannelResponse PauseChannel(PauseChannelRequest request);

        ResumeChannelResponse ResumeChannel(ResumeChannelRequest request);

    }
}
