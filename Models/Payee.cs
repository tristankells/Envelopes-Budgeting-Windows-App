using Envelopes.Common;

namespace Envelopes.Models {
    public class Payee : Model
    {
        #region Fields

        private int id;
        private string name;

        #endregion

        #region Properties

        public int Id {
            get => id;
            set => SetPropertyValue(ref id, value, nameof(Id));
        }

        public string Name {
            get => name;
            set => SetPropertyValue(ref name, value, nameof(Name));
        }

        #endregion
    }
}