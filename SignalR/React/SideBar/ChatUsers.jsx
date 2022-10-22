import React from 'react';
import { Card } from 'react-bootstrap';
import PropTypes from 'prop-types';

// ChatUsers
const ChatUsers = ({ userComponents, searchUsers }) => {
    const searchUser = (e) => {
        let userToSearch = e.target.value;
        searchUsers(userToSearch);
    };

    return (
        <>
            <Card>
                <Card.Body className="p-0">
                    <div className="tab-content">
                        <div className="tab-pane show active">
                            <div className="app-search p-3">
                                <div className="form-group position-relative">
                                    <input
                                        type="text"
                                        className="form-control"
                                        placeholder="People, groups & messages..."
                                        onKeyUp={searchUser}
                                    />
                                    <span className="mdi mdi-magnify search-icon"></span>
                                </div>
                            </div>
                            {userComponents}
                        </div>
                    </div>
                </Card.Body>
            </Card>
        </>
    );
};

ChatUsers.propTypes = {
    userComponents: PropTypes.arrayOf(PropTypes.shape([])).isRequired,
    searchUsers: PropTypes.func.isRequired,
};

export default ChatUsers;
