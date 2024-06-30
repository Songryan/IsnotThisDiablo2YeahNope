using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ViewModel
{
    public class ViewModelBase : MonoBehaviour
    {
        #region PropChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}