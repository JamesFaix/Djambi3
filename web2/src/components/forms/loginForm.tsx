import * as React from 'react';
import { connect } from 'react-redux';
import { Dispatch } from 'redux';
import { LoginRequest } from '../../api/model';
import * as ThunkActions from '../../thunkActions';
import Styles from '../../styles/styles';
import SectionHeader from '../sections/sectionHeader';

interface LoginFormProps {
    submit: (formData: LoginRequest) => void
}

interface LoginFormState {
    username : string,
    password : string
}

class loginForm extends React.Component<LoginFormProps, LoginFormState> {
    constructor(props : LoginFormProps) {
        super(props);
        this.state = {
            username: "",
            password: ""
        };
    }

    private getFormDataFromState() : LoginRequest {
        return {
            username: this.state.username,
            password: this.state.password
        };
    }

    render() {
        return (
            <div style={Styles.pageContainer()}>
                <SectionHeader text="Log in"/>
                <table>
                    <tbody>
                        <tr>
                            <td style={Styles.noBorder()}>
                                Username
                            </td>
                            <td style={Styles.noBorder()}>
                                <input
                                    type="text"
                                    value={this.state.username}
                                    onChange={e => this.setState({ username: e.target.value })}
                                >
                                </input>
                            </td>
                        </tr>
                        <tr>
                            <td style={Styles.noBorder()}>
                                Password
                            </td>
                            <td style={Styles.noBorder()}>
                                <input
                                    type="password"
                                    value={this.state.password}
                                    onChange={e => this.setState({ password: e.target.value })}
                                >
                                </input>
                            </td>
                        </tr>
                    </tbody>
                </table>
                <button style={Styles.smallTopMargin()}
                    onClick={() => this.props.submit(this.getFormDataFromState())}
                >
                    Log in
                </button>
            </div>
        );
    }
}

const mapDispatchToProps = (dispatch : Dispatch) => {
    return {
        submit: (formData : LoginRequest) => ThunkActions.login(formData)(dispatch)
    };
};

const LoginForm = connect(null, mapDispatchToProps)(loginForm);

export default LoginForm;