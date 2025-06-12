using Application.UseCases.Labels;
namespace Application.Interfaces.Repositories;

public interface ILabelRepository
{
    VmiLabelDto GetVmiLabel(string Pid);
}
