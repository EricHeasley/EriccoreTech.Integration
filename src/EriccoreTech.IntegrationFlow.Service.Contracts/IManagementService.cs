using EriccoreTech.IntegrationFlow.Model;

namespace EriccoreTech.IntegrationFlow.Service.Contracts {
    public interface IManagementService {

        ListChannelsResponse ListChannels(ListChannelsRequest request);

        PauseChannelResponse PauseChannel(PauseChannelRequest request);

        ResumeChannelResponse ResumeChannel(ResumeChannelRequest request);

    }
}
