using System.ComponentModel; // INotifyPropertyChanged �������̽��� ����ϱ� ���� System.ComponentModel ���ӽ����̽��� �����մϴ�.

namespace ViewModel
{
    // ��� ViewModel Ŭ������ �⺻ Ŭ������ ���� ViewModelBase Ŭ�����Դϴ�.
    public class ViewModelBase : INotifyPropertyChanged // INotifyPropertyChanged �������̽��� �����Ͽ� �Ӽ� ���� �˸��� �����մϴ�.
    {
        #region PropChanged
        // PropertyChanged �̺�Ʈ�� �Ӽ��� ����� �� �߻��մϴ�.
        public event PropertyChangedEventHandler PropertyChanged;

        // OnPropertyChanged �޼���� �Ӽ� ���� ����Ǿ��� �� PropertyChanged �̺�Ʈ�� �߻���ŵ�ϴ�.
        protected virtual void OnPropertyChanged(string propertyName)
        {
            // PropertyChanged �̺�Ʈ �ڵ鷯�� null�� �ƴ� ���, �̺�Ʈ�� �߻���ŵ�ϴ�.
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
