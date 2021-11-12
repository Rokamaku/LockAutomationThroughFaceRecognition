import * as React from 'react';
import { FaceRectangle, PersonEntity } from 'src/ServiceProxies';
import { Rect, Text } from 'react-konva';


interface ITagPersonProps {
  faceRectangle: FaceRectangle;
  person: PersonEntity;
}

interface ITagPersonState {

}

export class TagPerson extends React.Component<ITagPersonProps, ITagPersonState> {
  constructor(props: ITagPersonProps) {
    super(props);
    this.state = {

    }
  }

  public render() : JSX.Element {
    const { faceRectangle, person} = this.props;
    return (
      <React.Fragment>
        <Rect
          x={faceRectangle.left}
          y={faceRectangle.top}
          width={faceRectangle.width}
          height={faceRectangle.height}
          stroke="white"
          strokeWidth={2}
          shadowBlur={5}
        />
        <Text text={this.getDisplayText()}
          x={this.calculateTextX()}
          y={this.calculateTextY()} 
          fontSize={18}
          fontStyle="bold"
          fill="blue"
        />
      </React.Fragment>
    )
  }

  private calculateTextX = (): number => {
    const { faceRectangle } = this.props;
    const displayText = this.getDisplayText();
    return (faceRectangle.left + faceRectangle.width / 2) - (displayText.length * 4);
  }

  private calculateTextY = (): number => {
    const { faceRectangle } = this.props;
    return faceRectangle.top + faceRectangle.height + 8;
  }

  private getDisplayText = (): string => {
    const { person } = this.props;
    return person ? `${person.firstName} ${person.lastName}` : 'Unknown';
  }
}