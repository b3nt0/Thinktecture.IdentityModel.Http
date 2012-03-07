
namespace Thinktecture.Samples.Resources.Data
{
    public interface IConsultantsRepository
    {
        Consultants GetAll();
        int Add(Consultant consultant);
        void Update(Consultant consultant);
    }
}
