import React, { useEffect, useRef } from 'react';
import { useForm } from 'react-hook-form';
import { Card, Row, Col } from 'react-bootstrap';
import SimpleBar from 'simplebar-react';
import { IoMdSend } from 'react-icons/io';
import PropTypes from 'prop-types';
import { FormInput } from './FormInput';
import Loader from './Loader';
import chatSchema from './chatSchema';
import './chat.css';

// ChatArea
const ChatArea = ({ userMessages, isLoading, sendChatMessage }) => {
    const methods = useForm({ resolver: chatSchema });
    const {
        handleSubmit,
        register,
        control,
        formState: { errors },
    } = methods;

    const messagesEndRef = useRef(null);
    const scrollToBottom = () => {
        messagesEndRef.current.scrollIntoView({
            behavior: 'smooth',
            block: 'end',
            inline: 'nearest',
        });
    };
    useEffect(scrollToBottom);

    return (
        <Card>
            <Card.Body className="position-relative px-0 pb-0">
                {isLoading && <Loader />}
                <SimpleBar className="chatarea-simplebar-chats">
                    <ul className="conversation-list px-3">{userMessages.messageComponents}</ul>
                    <div ref={messagesEndRef} />
                </SimpleBar>
                <Row className="px-3 pb-3">
                    <Col>
                        <div className="mt-2 bg-light p-3 rounded">
                            <form noValidate name="chat-form" id="chat-form" onSubmit={handleSubmit(sendChatMessage)}>
                                <div className="row">
                                    <div className="col mb-2 mb-sm-0">
                                        <FormInput
                                            type="text"
                                            name="newMessage"
                                            className="border-0"
                                            placeholder="Enter your text"
                                            control={control}
                                            register={register}
                                            errors={errors}
                                        />
                                    </div>
                                    <div className="col-sm-auto">
                                        <div className="btn-group">
                                            <button
                                                type="submit"
                                                className="btn btn-success chat-send btn-block"
                                                key="newMessageSend">
                                                <IoMdSend />
                                            </button>
                                        </div>
                                    </div>
                                </div>
                            </form>
                        </div>
                    </Col>
                </Row>
            </Card.Body>
        </Card>
    );
};
ChatArea.propTypes = {
    userMessages: PropTypes.shape({ messageComponents: PropTypes.arrayOf(PropTypes.shape([])) }),
    isLoading: PropTypes.bool,
    sendChatMessage: PropTypes.func,
};

export default ChatArea;
