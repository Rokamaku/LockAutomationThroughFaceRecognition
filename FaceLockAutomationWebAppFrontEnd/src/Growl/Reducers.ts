import { SHOW_MESSAGE, CLEAR_MESSAGE } from './ActionTypes';

const initialState = {
  message: []
}

export default function GrowReducer(state = initialState, action: any) {
  switch (action.type) {
    case SHOW_MESSAGE:
      const messages = [];
      messages.push(action.message);
    return {
      messages
    };
    case CLEAR_MESSAGE:
      return {
        message: []
      }
    default:
        return state;
  }
}