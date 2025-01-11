using Caliburn.Micro;
using SchacksMacroManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchacksMacroManager.ViewModels
{
    public class UserViewModel : PropertyChangedBase
    {
        public delegate void ActiveUserChanged(UserViewModel sender);
        public event ActiveUserChanged OnActiveUserChanged;
        public delegate void UserRemoved(UserViewModel sender);
        public event UserRemoved OnUserRemoved;
        public User User { get; set; }
        public bool CheckBoxIsEnabled => !IsActive;
        private bool _isActive;
        public bool IsActive
        {
            get => _isActive;
            set
            {
                if (value != _isActive)
                {
                    if(!_isActive)
                        OnActiveUserChanged.Invoke(this);
                    _isActive = value;
                    NotifyOfPropertyChange(() => IsActive);
                    NotifyOfPropertyChange(() => CheckBoxIsEnabled);
                }
            }
        }

        public string Name {
            get
            {
                return User.Name;
            }
            set 
            {
                User.Name = value;
            }
        }
        public UserViewModel(User user)
        {
            User = user;
        }

        public void RemoveUser()
        {
            OnUserRemoved?.Invoke(this);
            
        }
    }
}
