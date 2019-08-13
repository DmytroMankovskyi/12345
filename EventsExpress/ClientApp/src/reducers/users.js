import initialState from '../store/initialState';
import { GET_USERS_ERROR, GET_USERS_PENDING, GET_USERS_SUCCESS } from '../actions/users';
import { blockUser, unBlockUser, changeUserRole } from '../actions/user';

export const reducer = (state = initialState.users, action) => {
    switch(action.type) {
        case GET_USERS_SUCCESS:
            return{
                ...state,
                isPending: false,
                data: action.payload
            }

        case GET_USERS_PENDING:
            return{
                ...state,
                isPending: action.payload
            }

        case GET_USERS_ERROR:
            return {
                ...state,
                isError: action.payload,
                isPending: false
            }

        case blockUser.UPDATE: {
            let newState = { ...state };
            newState.data.items = state.data.items.map((item) => {
                if (item.id === action.payload) {
                    let updatedItem = item;
                    updatedItem.isBlocked = true;
                    return updatedItem;
                }
                return item;
            });
            return newState;
        }

        case unBlockUser.UPDATE: {
            let newState = { ...state };
            newState.data.items = state.data.items.map((item) => {
                if (item.id === action.payload) {
                    let updatedItem = item;
                    updatedItem.isBlocked = false;
                    return updatedItem;
                }
                return item;
            });
            return newState;
        }
            
        case changeUserRole.SET_EDITED: {
            return {
                ...state,
                editedUser: action.payload
            }
        }

        case changeUserRole.UPDATE: {
            let newState = { ...state };
            newState.data.items = state.data.items.map((item) => {
                if (item.id === action.payload.userId) {
                    let updatedItem = item;
                    updatedItem.role = action.payload.newRole;
                    return updatedItem;
                }
                return item;
            });
            return newState;
        }

        default:
            return state;
    }
}