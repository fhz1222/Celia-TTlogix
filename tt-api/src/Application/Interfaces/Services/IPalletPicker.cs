namespace Application.Interfaces.Services;

public interface IPalletPicker
{
    Task Pick(string palletId, string outboundJob, string? parentPalletId);
}
