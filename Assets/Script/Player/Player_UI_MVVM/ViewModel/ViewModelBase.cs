using System.ComponentModel; // INotifyPropertyChanged 인터페이스를 사용하기 위해 System.ComponentModel 네임스페이스를 포함합니다.

namespace ViewModel
{
    // 모든 ViewModel 클래스의 기본 클래스로 사용될 ViewModelBase 클래스입니다.
    public class ViewModelBase : INotifyPropertyChanged // INotifyPropertyChanged 인터페이스를 구현하여 속성 변경 알림을 제공합니다.
    {
        #region PropChanged
        // PropertyChanged 이벤트는 속성이 변경될 때 발생합니다.
        public event PropertyChangedEventHandler PropertyChanged;

        // OnPropertyChanged 메서드는 속성 값이 변경되었을 때 PropertyChanged 이벤트를 발생시킵니다.
        protected virtual void OnPropertyChanged(string propertyName)
        {
            // PropertyChanged 이벤트 핸들러가 null이 아닌 경우, 이벤트를 발생시킵니다.
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
