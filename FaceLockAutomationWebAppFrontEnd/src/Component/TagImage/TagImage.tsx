import * as React from 'react';
import { FaceLogEntity, PersonEntity } from 'src/ServiceProxies';
import { Layer, Stage } from 'react-konva';
import { OriginImage } from './OriginImage';
import { TagPerson } from './TagPerson';

interface ITagImageProps {
  faceLog: FaceLogEntity;
  persons: PersonEntity[];
}

interface ITagImageState {
  width?: number;
  height?: number;
}

export class TagImage extends React.Component<ITagImageProps, ITagImageState> {

  constructor(props: ITagImageProps) {
    super(props);
    this.state = {
      width: 0,
      height: 0
    }
  }

  public componentDidMount() {
    
  }

  public render() : JSX.Element {
    const { faceLog, persons } = this.props;

    const faceLogRectangles = faceLog.faceRectangles;
    const personsInLog = faceLog.persons.map(psId => persons.filter(ps => ps.objectId === psId)[0]);

    return (
      <Stage width={this.state.width} height={this.state.height} >
        <Layer > 
          <OriginImage imageUrl={faceLog.file.uri} setImgDim={this.setImgDimension}/>
          {
            faceLogRectangles.map((fl, idx) => 
              <TagPerson key={personsInLog[idx] ? personsInLog[idx].id : idx} 
              faceRectangle={fl} person={personsInLog[idx]} 
              />
            )
          }        
        </Layer>
      </Stage>
    )
  }

  private setImgDimension = (width: number, height: number) => {
    this.setState({
      width,
      height
    })
  }
}