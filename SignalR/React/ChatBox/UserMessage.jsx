import React, { useState } from 'react';
import { Dropdown } from 'react-bootstrap';
import classnames from 'classnames';
import { IoIosArrowDropdown } from 'react-icons/io';
import MessageService from '../../services/messageService';
import PropTypes from 'prop-types';
import logger from 'sabio-debug';

const _logger = logger.extend('UserMessage');

const UserMessage = ({ message, currentUser, recipientId, senderAvatar, senderName, dateSent, id }) => {
    const [editMsg, setEditMsg] = useState();
    const [toggleEdit, setToggleEdit] = useState(false);

    let utcToLocal = new Date(`${dateSent}Z`).toLocaleTimeString('en-US', {
        hour: 'numeric',
        minute: '2-digit',
    });
    const onDeleteClicked = (e) => {
        const idDel = e.target.id;
        MessageService.deleteById(idDel).then(delMsgSuccess).catch(delMsgError);
    };

    const delMsgSuccess = (response) => {
        _logger(response);
    };

    const delMsgError = (err) => {
        _logger(err);
    };
    const onEditClicked = () => {
        setToggleEdit(!toggleEdit);
    };
    const messageEdit = (e) => {
        setEditMsg((prevState) => {
            let newEdit = { ...prevState };
            newEdit = e.target.value;
            return newEdit;
        });
    };
    const handleSave = (e) => {
        let msgId = e.target.id;
        let newMsg = { message: editMsg, subject: 'subject' };
        MessageService.update(newMsg, msgId).then(editMsgSuccess).catch(editMsgError);
        setToggleEdit(!toggleEdit);
    };

    const editMsgSuccess = (response) => {
        _logger(response);
    };

    const editMsgError = (err) => {
        _logger(err);
    };
    return (
        <React.Fragment>
            <li className={classnames('clearfix', { odd: recipientId !== currentUser.id })}>
                <div className="chat-avatar">
                    <img src={senderAvatar} className="rounded" alt="" />
                    <i>{utcToLocal}</i>
                </div>

                <div className="conversation-text">
                    <div className="ctext-wrap">
                        <i>{senderName}</i>

                        {toggleEdit ? <p>{editMsg}</p> : message && <p>{message}</p>}
                        {toggleEdit && (
                            <div>
                                <textarea
                                    type="text"
                                    name="msgForEdit"
                                    className="edit-message"
                                    defaultValue={message}
                                    onChange={messageEdit}></textarea>
                                <button className="btn btn-link" id={id} color="info" onClick={handleSave}>
                                    Save
                                </button>
                            </div>
                        )}
                    </div>
                    <div className="float-end">
                        {recipientId !== currentUser.id && (
                            <Dropdown className="conversation-actions" align="end">
                                <Dropdown.Toggle variant="link" className="btn btn-md btn-link arrow-none shadow-none">
                                    <IoIosArrowDropdown />
                                </Dropdown.Toggle>
                                <Dropdown.Menu>
                                    <Dropdown.Item id={id} onClick={onDeleteClicked}>
                                        Delete
                                    </Dropdown.Item>
                                    <Dropdown.Item onClick={onEditClicked}>Edit</Dropdown.Item>
                                </Dropdown.Menu>
                            </Dropdown>
                        )}
                    </div>
                </div>
            </li>
        </React.Fragment>
    );
};
UserMessage.propTypes = {
    message: PropTypes.string.isRequired,
    currentUser: PropTypes.shape({ id: PropTypes.number }),
    recipientId: PropTypes.number.isRequired,
    senderAvatar: PropTypes.string.isRequired,
    senderName: PropTypes.string.isRequired,
    dateSent: PropTypes.string.isRequired,
    id: PropTypes.number,
    onClickDelete: PropTypes.func,
    onEditMessage: PropTypes.func,
};
export default UserMessage;
