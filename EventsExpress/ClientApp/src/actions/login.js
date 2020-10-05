import EventsExpressService from '../services/EventsExpressService';
import { func } from 'prop-types';
import { initialConnection } from './chat';
import { getUnreadMessages } from './chats';

export const SET_LOGIN_PENDING = "SET_LOGIN_PENDING";
export const SET_LOGIN_SUCCESS = "SET_LOGIN_SUCCESS";
export const SET_LOGIN_ERROR = "SET_LOGIN_ERROR";
export const SET_USER = "SET_USER";

const api_serv = new EventsExpressService();

export default function login(email, password) {
  return dispatch => {
    dispatch(setLoginPending(true));
    const res = api_serv.setLogin({ Email: email, Password: password });
    loginResponseHandler(res, dispatch);
  }
}

export function loginGoogle(tokenId, email, name, imageUrl) {
  return dispatch => {
    dispatch(setLoginPending(true));

    const res = api_serv.setGoogleLogin({
      TokenId: tokenId,
      Email: email,
      Name: name,
      PhotoUrl: imageUrl
    });

    loginResponseHandler(res, dispatch);
  }
}

export function loginFacebook(email, name) {
  return dispatch => {
    dispatch(setLoginPending(true));
    const res = api_serv.setFacebookLogin({ Email: email, Name: name });
    loginResponseHandler(res, dispatch);
  }
}

export function loginTwitter(data) {
  return dispatch => {
    dispatch(setLoginPending(true));
    const res = api_serv.setTwitterLogin({
      TokenId: data.oauth_token,
      TokenSecret: data.oauth_token_secret,
      UserId: data.user_id,
      Email: data.email,
      Name: typeof data.name !== 'undefined' ? data.name : data.screen_name,
      PhotoUrl: data.image_url,
    });
    loginResponseHandler(res, dispatch);
  }
}

export function setUser(data) {
  return {
    type: SET_USER,
    payload: data
  };
}

export function setLoginPending(isLoginPending) {
  return {
    type: SET_LOGIN_PENDING,
    isLoginPending
  };
}

export function setLoginSuccess(isLoginSuccess) {
  return {
    type: SET_LOGIN_SUCCESS,
    isLoginSuccess
  };
}

export function setLoginError(loginError) {
  return {
    type: SET_LOGIN_ERROR,
    loginError
  };
}

const loginResponseHandler = (res, dispatch) => {
  res.then(response => {
    if (response.error == null) {
      dispatch(setUser(response));
      dispatch(setLoginSuccess(true));
      localStorage.setItem('token', response.token);
      localStorage.setItem('id', response.id);
      dispatch(initialConnection());
      dispatch(getUnreadMessages(response.id));
    } else {
      dispatch(setLoginError(response.error));
    }
  });
};
