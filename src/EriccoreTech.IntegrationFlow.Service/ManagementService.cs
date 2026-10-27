using EriccoreTech.IntegrationFlow.Management;
using EriccoreTech.IntegrationFlow.Core;
using EriccoreTech.IntegrationFlow.Contracts;

namespace EriccoreTech.IntegrationFlow.Service {
    public sealed class ManagementService : IManagementService {

        private readonly ControlBus _controlBus;

        public ManagementService(ControlBus controlBus) {
            _controlBus = controlBus;
        }

        public ListChannelsResponse ListChannels(ListChannelsRequest request) {
            return new ListChannelsResponse(_controlBus.Snapshot());
        }

        public PauseChannelResponse PauseChannel(PauseChannelRequest request) {
            if (request is null) {
                throw new ArgumentNullException(nameof(request));
            }

            _controlBus.Pause(request.Topic);
            return new PauseChannelResponse(request.Topic, true);
        }

        public ResumeChannelResponse ResumeChannel(ResumeChannelRequest request) {
            if (request is null) {
                throw new ArgumentNullException(nameof(request));
            }

            _controlBus.Resume(request.Topic);
            return new ResumeChannelResponse(request.Topic, false);
        }

    }
}
