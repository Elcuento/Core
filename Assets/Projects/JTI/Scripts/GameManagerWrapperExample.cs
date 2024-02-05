using JTI.Scripts.Managers;

namespace JTI.Scripts
{
    public class GameManagerWrapperExample : GameManager
    {
        protected override void InstallControllers()
        {
            AddController<DataBaseController>()
                 .SetSettings(new DataBaseController.DataBaseControllerSettings("Data/"))
                 .Install();

            AddController<TimeController>()
                .SetSettings(new TimeController.TimeControllerSettings())
                .Install();

            AddController<ProfileController>()
                .SetSettings(new ProfileController.ProfileControllerSettings())
                .Install();
        }

    }
}