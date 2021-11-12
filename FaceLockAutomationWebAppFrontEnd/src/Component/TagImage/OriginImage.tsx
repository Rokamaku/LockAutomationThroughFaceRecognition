import * as React from 'react';
import { Image } from 'react-konva';

interface IOriginImageProps {
  imageUrl: string;
  setImgDim: (width: number, height: number) => void;
}

interface IOriginImageState {
  image?: any;
}

export class OriginImage extends React.Component<IOriginImageProps, IOriginImageState> {
  constructor(props: IOriginImageProps) {
    super(props);
    this.state = {
      image: null
    }
  }

  public componentDidMount() {
    const image = new (window as any).Image();
    image.src = this.props.imageUrl;

    image.onload = () => {
      this.props.setImgDim(image.width, image.height);
      this.setState({
        image
      })
    }
  }

  public render() : JSX.Element {
    return <Image image={this.state.image}/>
  }
}