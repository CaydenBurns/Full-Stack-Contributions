import { yupResolver } from '@hookform/resolvers/yup';
import * as yup from 'yup';

const chatSchema = yupResolver(
    yup.object().shape({
        newMessage: yup.string().required('Please enter your messsage'),
    })
);

export default chatSchema;
