import { SHOW_MESSAGE, CLEAR_MESSAGE } from './ActionTypes';


export function showGrowl(message: any) {
  return {
    type: SHOW_MESSAGE,
    message
  }
}

export function clearGrowl() {
  return {
    type: CLEAR_MESSAGE,
  }
}