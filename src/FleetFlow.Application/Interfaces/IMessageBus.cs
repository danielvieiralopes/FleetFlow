namespace FleetFlow.Application.Interfaces;

public interface IMessageBus
{
    void Publish(string queue, byte[] message);
}