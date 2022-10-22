import React, { useEffect, useState } from 'react';
import classnames from 'classnames';
import SimpleBar from 'simplebar-react';
import { Link } from 'react-router-dom';
import PropTypes from 'prop-types';
import './chat.css';

const UserCard = ({ user, onUserSelect, selectedUser }) => {
    const [userData, setUserData] = useState({ message: '', dateSent: '', msgId: '' });

    useEffect(() => {
        mapUserData();
    }, []);

    const mapUserData = () => {
        setUserData((...prevState) => {
            let newData = { ...prevState };
            newData.message = user.conversation.map((e) => e.message);
            newData.dateSent = user.conversation.map((e) => e.dateSent);
            newData.msgId = user.conversation.map((e) => e.id);
            return newData;
        });
    };

    const activateUser = (user) => {
        if (onUserSelect) {
            onUserSelect(user);
        }
    };
    return (
        <SimpleBar className="usercard-simplebar-users">
            <Link
                to="#"
                key={userData.id}
                className="text-body"
                onClick={() => {
                    activateUser(user);
                }}>
                <div
                    className={classnames('d-flex', 'align-items-start', 'mt-1', 'p-2', {
                        'bg-light': user?.recipientName === selectedUser?.recipientName,
                    })}>
                    <div>
                        <img
                            src={user.recipientAvatar}
                            className="me-2 rounded-circle avatar avatar-md avatar-indicators"
                            alt=""
                        />
                    </div>

                    <div className="w-100 overflow-hidden">
                        <h5 className="mt-0 mb-0 font-14">
                            <span className="float-end text-muted font-12">
                                {new Date(`${userData.dateSent}Z`).toLocaleTimeString('en-US', {
                                    hour: 'numeric',
                                    minute: '2-digit',
                                })}
                            </span>
                            {user.recipientName}
                        </h5>
                        <p className="mt-1 mb-0 text-muted font-14">
                            <span className="w-75 mx-1">{userData.message}</span>
                        </p>
                    </div>
                </div>
            </Link>
        </SimpleBar>
    );
};

UserCard.propTypes = {
    user: PropTypes.shape({
        conversation: PropTypes.arrayOf(PropTypes.shape([])).isRequired,
        recipientName: PropTypes.string.isRequired,
        recipientAvatar: PropTypes.string.isRequired,
    }).isRequired,
    onUserSelect: PropTypes.func.isRequired,
    selectedUser: PropTypes.shape({ recipientName: PropTypes.string }),
};

export default UserCard;
