import * as React from 'react';
import { Button, Header, Segment, Menu, Icon } from 'semantic-ui-react'


export interface IHeaderProps {
  clickPanel: () => void;
}


export interface IHeaderState {
  
}

export default class Topbar extends React.Component<IHeaderProps, IHeaderState> {
  constructor(props: IHeaderProps) {
    super(props);
    this.state = {

    }
  }

  public render(): JSX.Element {
    return (
      <Menu secondary attached="top">
        <Menu.Item onClick={this.clickShowPanel} >
          <Icon name="sidebar" />
        </Menu.Item>          
      </Menu>
    )
  }

  public clickShowPanel = () => {
    this.props.clickPanel();
  }
}