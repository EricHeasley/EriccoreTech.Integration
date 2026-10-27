using EriccoreTech.Integration.Core;

namespace EriccoreTech.Integration.Contracts {
    public interface IManagementService {

        ListChannelsResponse ListChannels(ListChannelsRequest request);

        PauseChannelResponse PauseChannel(PauseChannelRequest request);

        ResumeChannelResponse ResumeChannel(ResumeChannelRequest request);

    }
}
